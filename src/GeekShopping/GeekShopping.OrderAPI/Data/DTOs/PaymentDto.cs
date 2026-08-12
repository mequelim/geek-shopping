using GeekShopping.MessageBus.Classes;

namespace GeekShopping.OrderAPI.Data.DTOs
{
    /// <summary>
    /// Represents the data transfer object for payment information, including details such as order ID, customer information, card details, and purchase amount.
    /// </summary>
    public class PaymentDto : BaseMessage
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order associated with the payment.
        /// </summary>
        public Guid OrderId { get; set; }

        /// <summary>
        /// Gets or sets the name associated with the payment.
        /// </summary>
        /// <value>A string representing the name of the individual or entity making the payment.</value>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address associated with the payment.
        /// </summary>
        /// <value>A <see cref="string"/> representing the email address of the payer.</value>
        public required string Email { get; set; }

        /// <summary>
        /// Gets or sets the credit card number associated with the payment.
        /// </summary>
        /// <remarks>Ensure that this property is securely handled and masked in logs or UI to protect sensitive information.</remarks>
        public required string CardNumber { get; set; }

        /// <summary>
        /// Gets or sets the CVV (Card Verification Value) of the payment card.
        /// </summary>
        /// <remarks>The CVV is a security feature for credit or debit card transactions, providing an additional layer of authentication.</remarks>
        public required string Cvv { get; set; }

        /// <summary>
        /// Gets or sets the expiration date of the payment card in the format MM/YYYY.
        /// </summary>
        public required string ExpiryMonthYear { get; set; }

        /// <summary>
        /// Gets or sets the total amount of the purchase.
        /// </summary>
        /// <value>A <see cref="decimal"/> representing the total cost of the order.</value>
        public decimal PurchaseAmount { get; set; }
    }
}