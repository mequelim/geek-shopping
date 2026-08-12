using GeekShopping.PaymentAPI.Data.DTOs;
using GeekShopping.PaymentAPI.RabbitMQ.Sender.Interface;
using GeekShopping.PaymentProcessor.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQ.PaymentConsumer
{
    /// <summary>
    /// A background service that consumes payment messages from a RabbitMQ queue, processes the payment and publishes the result to a result queue.
    /// </summary>
    public class RabbitMqPaymentConsumer(IConfiguration configuration, IProcessorPayment processorPayment, IRabbitMqMessageSender rabbitMqMessageSender) : BackgroundService
    {
        private readonly IConfiguration _configuration = configuration
                                                         ?? throw new ArgumentNullException(nameof(configuration));
        private readonly IProcessorPayment _processorPayment = processorPayment
                                                               ?? throw new ArgumentNullException(nameof(processorPayment));
        private readonly IRabbitMqMessageSender _rabbitMqMessageSender = rabbitMqMessageSender
                                                                         ?? throw new ArgumentNullException(nameof(rabbitMqMessageSender));
        private IConnection? _connection;
        private IChannel? _channel;

        // Methods:
        /// <summary>
        /// Initializes the RabbitMQ connection and channel and declares the queue to be consumed.
        /// </summary>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous initialization operation.</returns>
        private async Task InitializeAsync(CancellationToken cancellationToken)
        {
            IConfigurationSection rabbitConfigs = _configuration.GetSection("RabbitMQ");
            string queueName = rabbitConfigs["QueueName"] ?? throw new InvalidOperationException("RabbitMQ:QueueName is not configured.");
            ConnectionFactory factory = new()
            {
                HostName = rabbitConfigs["HostName"]!,
                UserName = rabbitConfigs["UserName"]!,
                Password = rabbitConfigs["Password"]!
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: cancellationToken
            );
        }

        /// <summary>
        /// Processes a payment message, invoking the payment processor and publishing the result to the payment result queue.
        /// </summary>
        /// <param name="paymentMessageDto">The payment message DTO containing order and payment details.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="paymentMessageDto"/> is null.</exception>
        private async Task ProcessPayment(PaymentMessageDto? paymentMessageDto)
        {
            ArgumentNullException.ThrowIfNull(paymentMessageDto);

            bool paymentStatus = _processorPayment.PaymentProcessor();
            UpdatePaymentResultMessage paymentResult = new()
            {
                PaymentStatus = paymentStatus,
                OrderId = paymentMessageDto.OrderId,
                Email = paymentMessageDto.Email
            };

            try
            {
                await _rabbitMqMessageSender.SendMessage(paymentResult);
            }
            catch(Exception exception)
            {
                Console.WriteLine($"An exception occurred when we tried to post a message on RabbitMQ: {exception}.");
                throw;
            }
        }

        /// <summary>
        /// Executes the background service, setting up the RabbitMQ consumer and processing incoming payment messages from the configured queue.
        /// </summary>
        /// <param name="stoppingToken">A cancellation token that signals when the service should stop.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous execution of the service.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the RabbitMQ channel is not properly initialized.</exception>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await InitializeAsync(stoppingToken);
            stoppingToken.ThrowIfCancellationRequested();

            if(_channel is null) throw new ArgumentNullException(nameof(_channel));

            IConfigurationSection rabbitConfigs = _configuration.GetSection("RabbitMQ");
            AsyncEventingBasicConsumer consumer = new(_channel);

            consumer.ReceivedAsync += async (_, evt) =>
            {
                try
                {
                    string content = Encoding.UTF8.GetString(evt.Body.ToArray());
                    Console.WriteLine($">>> Received payment message: {content}");

                    PaymentMessageDto? paymentMessageDto = JsonSerializer.Deserialize<PaymentMessageDto>(content);

                    await ProcessPayment(paymentMessageDto);
                    await _channel.BasicAckAsync(evt.DeliveryTag, multiple: false, stoppingToken);
                }
                catch(Exception exception)
                {
                    Console.WriteLine($">>>>> ERROR processing payment message: {exception}");
                }
            };

            await _channel.BasicConsumeAsync(
                rabbitConfigs["QueueName"]!,
                autoAck: false,
                consumer,
                cancellationToken: stoppingToken
            );
        }

        /// <summary>
        /// Asynchronously releases the RabbitMQ channel and connection resources.
        /// </summary>
        /// <returns>A task that represents the asynchronous disposal operation.</returns>
        public async ValueTask DisposeAsync()
        {
            if(_channel is not null) await _channel.DisposeAsync();
            if(_connection is not null) await _connection.DisposeAsync();

            base.Dispose();
        }
    }
}