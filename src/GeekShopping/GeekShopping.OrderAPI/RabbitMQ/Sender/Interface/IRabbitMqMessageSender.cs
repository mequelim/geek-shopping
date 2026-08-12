using GeekShopping.MessageBus.Classes;

namespace GeekShopping.OrderAPI.RabbitMQ.Sender.Interface
{
    /// <summary>
    /// Defines a contract for sending messages to RabbitMQ queues.
    /// Implementations of this interface are responsible for managing the process of publishing messages to the specified RabbitMQ queue.
    /// </summary>
    public interface IRabbitMqMessageSender
    {
        /// <summary>
        /// Sends a message to the specified RabbitMQ queue.
        /// </summary>
        /// <param name="baseMessage">
        /// The message object of type <see cref="BaseMessage"/> to be sent.
        /// It represents the payload to be delivered to the queue.
        /// </param>
        /// <param name="queueName">The name of the queue to which the message will be sent.</param>
        /// <returns>A task representing the asynchronous operation of sending the message to the RabbitMQ queue.</returns>
        Task SendMessage(BaseMessage baseMessage, string queueName);
    }
}