namespace GeekShopping.Email.Data.DTOs
{
    /// <summary>
    /// Represents the data transfer object for updating the payment result of an order.
    /// </summary>
    public class UpdatePaymentResultMessage
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order associated with the payment.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the payment.
        /// </summary>
        /// <value>A <see cref="string"/> representing the email address of the payer.</value>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the status indicating whether the payment was successful or not.
        /// </summary>
        public bool PaymentStatus { get; set; }
    }
}