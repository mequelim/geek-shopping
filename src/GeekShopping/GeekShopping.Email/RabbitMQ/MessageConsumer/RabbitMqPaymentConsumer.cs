using GeekShopping.Email.Data.DTOs;
using GeekShopping.Email.Repository.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.Email.RabbitMQ.MessageConsumer
{
    /// <summary>
    /// RabbitMqPaymentResultConsumer is a background service that consumes payment result messages from a RabbitMQ queue.
    /// It manages the initialization of the RabbitMQ connection, configuration of the consumer, and processing of incoming messages.
    /// </summary>
    public class RabbitMqPaymentConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration) : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory
                                                              ?? throw new ArgumentNullException(nameof(scopeFactory));
        private readonly IConfiguration _configuration = configuration
                                                         ?? throw new ArgumentNullException(nameof(configuration));
        private IConnection? _connection;
        private IChannel? _channel;
        private const string ExchangeName = "FanoutPaymentUpdateExchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";

        // Methods:
        /// <summary>
        /// Initializes the RabbitMQ connection and channel and declares the queue to be consumed.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests during initialization.</param>
        /// <returns>A task representing the asynchronous initialization operation.</returns>
        private async Task InitializeAsync(CancellationToken cancellationToken)
        {
            IConfigurationSection rabbitConfigs = _configuration.GetSection("RabbitMQ");
            ConnectionFactory factory = new()
            {
                HostName = rabbitConfigs["HostName"]!,
                UserName = rabbitConfigs["UserName"]!,
                Password = rabbitConfigs["Password"]!
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            //? Creating an exchange:
            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Direct,
                durable: true,
                cancellationToken: cancellationToken
            );

            //? Getting the queue name dynamically:
            /* QueueDeclareOk queueDeclareResult = await _channel.QueueDeclareAsync(cancellationToken: cancellationToken);
            _queueName = queueDeclareResult.QueueName; */
            // queueName = (await _channel.QueueDeclareAsync(cancellationToken: cancellationToken)).QueueName;

            //? Creating a queue:
            await _channel.QueueDeclareAsync(
                queue: PaymentEmailUpdateQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            //? Creating the binding:
            await _channel.QueueBindAsync(
                queue: PaymentEmailUpdateQueueName,
                exchange: ExchangeName,
                routingKey: "PaymentEmail",
                cancellationToken: cancellationToken
            );
        }

        /// <summary>
        /// Updates the payment status of an order by logging the payment result message to the email repository.
        /// </summary>
        /// <param name="message">The message containing details about the order payment status to be updated.</param>
        /// <param name="emailRepository">The repository instance used for logging the email corresponding to the payment result.</param>
        /// <returns>A task representing the asynchronous operation for updating the payment status.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="message"/> parameter is null.</exception>
        private static async Task ProcessLog(UpdatePaymentResultMessage message, IEmailRepository emailRepository)
        {
            ArgumentNullException.ThrowIfNull(message);

            try
            {
                await emailRepository.LogEmail(message);
            }
            catch(Exception exception)
            {
                Console.WriteLine($"An exception occurred when updating order payment status: {exception}.");
                throw;
            }
        }

        /// <summary>
        /// Executes the background service for consuming payment result messages from the RabbitMQ queue.
        /// </summary>
        /// <param name="stoppingToken">A token to monitor for cancellation requests to stop execution.</param>
        /// <returns>A task that represents the execution of the message consumption operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the RabbitMQ channel is not initialized.</exception>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeAsync(stoppingToken);
            stoppingToken.ThrowIfCancellationRequested();

            if(_channel is null) throw new ArgumentNullException(nameof(_channel));

            AsyncEventingBasicConsumer consumer = new(_channel);

            consumer.ReceivedAsync += async (_, evt) =>
            {
                try
                {
                    string content = Encoding.UTF8.GetString(evt.Body.ToArray());

                    Console.WriteLine($">>> Received payment result message: {content}");

                    UpdatePaymentResultMessage? updatePaymentResultMessage = JsonSerializer.Deserialize<UpdatePaymentResultMessage>(content);

                    await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

                    IEmailRepository emailRepository = scope.ServiceProvider.GetRequiredService<IEmailRepository>();

                    await ProcessLog(updatePaymentResultMessage!, emailRepository);
                    await _channel.BasicAckAsync(evt.DeliveryTag, multiple: false, stoppingToken);
                }
                catch(Exception exception)
                {
                    Console.WriteLine($">>>>> Error processing payment result message: {exception}");
                }
            };

            await _channel.BasicConsumeAsync(
                queue: PaymentEmailUpdateQueueName,
                autoAck: false,
                consumer,
                cancellationToken: stoppingToken
            );
        }

        /// <summary>
        /// Asynchronously releases the RabbitMQ channel and connection resources.
        /// </summary>
        /// <returns>A task representing the asynchronous disposal operation.</returns>
        public async ValueTask DisposeAsync()
        {
            if(_channel is not null) await _channel.DisposeAsync();
            if(_connection is not null) await _connection.DisposeAsync();

            base.Dispose();
        }
    }
}