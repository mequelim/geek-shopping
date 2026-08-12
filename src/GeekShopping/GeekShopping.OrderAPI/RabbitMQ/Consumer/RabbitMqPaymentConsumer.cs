using GeekShopping.OrderAPI.Data.DTOs;
using GeekShopping.OrderAPI.Repository.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.OrderAPI.RabbitMQ.Consumer
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
        private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

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
                queue: PaymentOrderUpdateQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                cancellationToken: cancellationToken
            );

            //? Creating the binding:
            await _channel.QueueBindAsync(
                queue: PaymentOrderUpdateQueueName,
                exchange: ExchangeName,
                routingKey: "PaymentOrder",
                cancellationToken: cancellationToken
            );
        }

        /// <summary>
        /// Updates the payment status of an order based on the received payment result message.
        /// </summary>
        /// <param name="updatePaymentResultDto">The payment result DTO containing the order ID and payment status.</param>
        /// <param name="orderRepository">The repository interface used to persist the status update.</param>
        /// <returns>A task representing the asynchronous update operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="updatePaymentResultDto"/> is null.</exception>
        private static async Task UpdatePaymentStatus(UpdatePaymentResultDto? updatePaymentResultDto, IOrderRepository orderRepository)
        {
            ArgumentNullException.ThrowIfNull(updatePaymentResultDto);

            try
            {
                await orderRepository.UpdateOrderPaymentStatus(updatePaymentResultDto.OrderId, updatePaymentResultDto.PaymentStatus);
            }
            catch(Exception exception)
            {
                Console.WriteLine($"An exception occurred when updating order payment status: {exception}.");
                throw;
            }
        }

        /// <summary>
        /// Executes the background service, initializing the RabbitMQ connection, setting up the message consumer, and processing payment result messages from the configured queue.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that indicates when execution should stop.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous execution of the service.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the RabbitMQ channel is not properly initialized.</exception>
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

                    UpdatePaymentResultDto? dto = JsonSerializer.Deserialize<UpdatePaymentResultDto>(content);

                    await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();

                    IOrderRepository orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();

                    await UpdatePaymentStatus(dto, orderRepository);
                    await _channel.BasicAckAsync(evt.DeliveryTag, multiple: false, stoppingToken);
                }
                catch(Exception exception)
                {
                    Console.WriteLine($">>>>> Error processing payment result message: {exception}");
                }
            };

            await _channel.BasicConsumeAsync(
                queue: PaymentOrderUpdateQueueName,
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