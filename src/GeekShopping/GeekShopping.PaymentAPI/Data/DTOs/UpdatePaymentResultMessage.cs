using GeekShopping.MessageBus.Classes;

namespace GeekShopping.PaymentAPI.Data.DTOs
{
    /// <summary>
    /// Represents the data transfer object for updating the payment result of an order.
    /// </summary>
    public class UpdatePaymentResultMessage : BaseMessage
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order associated with the payment.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the payment.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the status of the payment for the associated order.
        /// </summary>
        public bool PaymentStatus { get; set; }
    }
}