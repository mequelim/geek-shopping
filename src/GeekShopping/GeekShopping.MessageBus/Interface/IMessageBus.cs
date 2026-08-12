using GeekShopping.MessageBus.Classes;

namespace GeekShopping.MessageBus.Interface
{
    /// <summary>
    /// Defines the contract for a message bus supporting message publishing functionality.
    /// </summary>
    public interface IMessageBus
    {
        /// <summary>
        /// Publishes a message to a specific topic on the message bus.
        /// </summary>
        /// <param name="message">The message to be published, inheriting from the BaseMessage class.</param>
        /// <param name="queueName">The name of the topic to which the message will be published.</param>
        /// <returns>A Task that represents the asynchronous operation of publishing the message.</returns>
        Task PublicMessage(BaseMessage message, string queueName);
    }
}