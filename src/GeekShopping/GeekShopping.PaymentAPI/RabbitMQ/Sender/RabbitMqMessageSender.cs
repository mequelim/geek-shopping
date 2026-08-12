using GeekShopping.PaymentAPI.RabbitMQ.Sender.Interface;
using GeekShopping.MessageBus.Classes;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace GeekShopping.PaymentAPI.RabbitMQ.Sender
{
    /// <summary>
    /// Responsible for sending messages to RabbitMQ queues.
    /// This class handles the connection management, message serialization, and ensures message delivery to the specified queue using the RabbitMQ client.
    /// </summary>
    public class RabbitMqMessageSender(IConfiguration configuration) : IRabbitMqMessageSender
    {
        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
        private IConnection? _connection;
        private const string ExchangeName = "FanoutPaymentUpdateExchange";
        private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName";
        private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

        // Methods:
        /// <summary>
        /// Checks if a RabbitMQ connection already exists.
        /// If no connection exists, it attempts to create a new one.
        /// </summary>
        /// <returns>An existing or newly established RabbitMQ connection.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the connection cannot be established after attempting to create one.</exception>
        private async Task<IConnection> GetOrConnectionExists()
        {
            if(_connection is not null) return _connection;

            await CreateConnection();

            return _connection
                   ?? throw new InvalidOperationException("Failed to establish a RabbitMQ connection!");
        }

        /// <summary>
        /// Establishes a new connection to RabbitMQ using the provided configuration settings.
        /// This method retrieves RabbitMQ configuration details, such as host name, username, and password, and uses them to create an asynchronous connection to the RabbitMQ server.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the RabbitMQ connection cannot be established due to invalid configuration or server issues.</exception>
        /// <remarks>This method is intended to be used internally to ensure a valid RabbitMQ connection is available before performing operations such as sending messages.</remarks>
        private async Task CreateConnection()
        {
            IConfigurationSection rabbitConfigs = configuration.GetSection("RabbitMQ");
            ConnectionFactory factory = new()
            {
                HostName = rabbitConfigs["HostName"]!,
                UserName = rabbitConfigs["UserName"]!,
                Password = rabbitConfigs["Password"]!
            };

            _connection = await factory.CreateConnectionAsync();
        }

        /// <summary>
        /// Converts an object into a byte array by serializing it to JSON format and encoding it as UTF-8.
        /// This method is primarily used to prepare messages for publishing to RabbitMQ queues.
        /// </summary>
        /// <param name="message">The object to be serialized and converted to a byte array.</param>
        /// <returns>A byte array representation of the serialized object.</returns>
        private static byte[] GetMessageAsByteArray(object message)
        {
            string json = JsonSerializer.Serialize(message, message.GetType(), JsonOptions);

            return Encoding.UTF8.GetBytes(json);
        }

        /// <summary>
        /// Sends a message to the specified RabbitMQ queue.
        /// </summary>
        /// <param name="baseMessage">The message to be sent to the queue.</param>
        /// <returns>A task representing the asynchronous operation of sending the message.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the RabbitMQ connection or channel cannot be established.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the baseMessage or queueName is null.</exception>
        public async Task SendMessage(BaseMessage baseMessage)
        {
            IConnection connection = await GetOrConnectionExists();

            await using IChannel channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(
                ExchangeName,
                ExchangeType.Direct,
                durable: true
            );

            await channel.QueueDeclareAsync(
                queue: PaymentEmailUpdateQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await channel.QueueDeclareAsync(
                queue: PaymentOrderUpdateQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await channel.QueueBindAsync(
                PaymentEmailUpdateQueueName,
                ExchangeName,
                routingKey: "PaymentEmail"
            );

            await channel.QueueBindAsync(
                PaymentOrderUpdateQueueName,
                ExchangeName,
                routingKey: "PaymentOrder"
            );

            byte[] body = GetMessageAsByteArray(baseMessage);

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: "PaymentEmail",
                basicProperties: new BasicProperties(),
                body: new ReadOnlyMemory<byte>(body),
                mandatory: false
            );

            await channel.BasicPublishAsync(
                exchange: ExchangeName,
                routingKey: "PaymentOrder",
                basicProperties: new BasicProperties(),
                body: new ReadOnlyMemory<byte>(body),
                mandatory: false
            );
        }
    }
}