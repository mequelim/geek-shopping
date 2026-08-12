namespace GeekShopping.MessageBus.Classes
{
    /// <summary>
    /// Represents the base structure for messages used within the message bus system.
    /// </summary>
    public class BaseMessage
    {
        /// <summary>
        /// Gets or sets the unique identifier for a message within the message bus system.
        /// </summary>
        public Guid BaseMessageId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp indicating when the message was created.
        /// </summary>
        public DateTime MessageCreateAt { get; set; }
    }
}