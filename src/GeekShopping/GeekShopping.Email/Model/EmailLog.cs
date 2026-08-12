namespace GeekShopping.Email.Model
{
    /// <summary>
    /// Represents a log entry for email-related activities associated with an order.
    /// </summary>
    /// <remarks>
    /// The <c>EmailLog</c> class is used to store information about email tracking and related details for specific orders and products.
    /// This entity is primarily used to persist log data in the database.
    /// </remarks>
    public sealed class EmailLog : BaseEntity
    {
        /// <summary>
        /// Gets or sets the email address associated with the email content.
        /// </summary>
        /// <remarks>
        /// This property contains the recipient's email address or the address used for email-related operations.
        /// It is a crucial element for identifying the target of the email communication within the system.
        /// </remarks>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the log details associated with the email activity.
        /// </summary>
        /// <remarks>
        /// This property contains detailed information about the specific actions or events related to an email, such as message content, statuses, or error descriptions.
        /// It is primarily used for tracking and troubleshooting purposes.
        /// </remarks>
        public string Log { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time when the email was sent.
        /// </summary>
        /// <remarks>
        /// This property captures the exact timestamp of when the email associated with the log entry was dispatched.
        /// It helps in tracking the chronological order of email operations and auditing purposes.
        /// </remarks>
        public DateTime SentDate { get; set; }
    }
}