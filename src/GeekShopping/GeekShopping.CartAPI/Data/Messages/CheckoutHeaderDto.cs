using GeekShopping.CartAPI.Data.DTOs;
using GeekShopping.MessageBus.Classes;

namespace GeekShopping.CartAPI.Data.Messages
{
    /// <summary>
    /// Represents the data transfer object for checkout header information used during the checkout process.
    /// This DTO consolidates the user, cart, and payment details necessary to complete a purchase transaction.
    /// </summary>
    public class CheckoutHeaderDto : BaseMessage
    {
        /// <summary>
        /// Gets the unique identifier for the user associated with the checkout process, linking the transaction to a specific account in the system.
        /// </summary>
        public required Guid UserId { get; init; }

        /// <summary>
        /// Gets or initializes the coupon code associated with the checkout, which may be used to apply discounts or promotions during the transaction.
        /// </summary>
        public string? CouponCode { get; init; }

        /// <summary>
        /// Gets or sets the total monetary amount associated with the purchase transaction, representing the sum of all items in the cart before any discounts or taxes
        /// are applied.
        /// </summary>
        public decimal PurchaseAmount { get; set; }

        /// <summary>
        /// Gets or sets the total discount amount applied to the cart during the checkout process.
        /// This value reflects the combined total of all discounts, including coupon codes and promotional offers.
        /// </summary>
        public decimal DiscountTotal { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user, typically used for personalization and identifying the primary individual associated with the checkout process.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the user initiating the checkout process.
        /// This property is used to personalize the checkout experience and may be required for billing or shipping purposes.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or initializes the date and time associated with the checkout header, representing the moment the checkout process is initiated.
        /// </summary>
        public DateTime DateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the phone number associated with the checkout process, used for communication or contact purposes.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address associated with the checkout process, typically used for communication and transaction updates.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the card number associated with the payment method used during the checkout process.
        /// </summary>
        public string CardNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expiration date of the card in the format MM/YY, used to verify the validity of the payment method during checkout.
        /// </summary>
        public string ExpiryMonthYear { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the card verification value (CVV), a security feature for credit or debit card transactions, often used to validate the authenticity of the card
        /// during the checkout process.
        /// </summary>
        public string Cvv { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of items in the cart, reflecting the combined quantity of all products added by the user.
        /// </summary>
        public int CartTotalItems { get; set; }

        /// <summary>
        /// Gets or sets the collection of details for the items in the shopping cart, including product information and quantity.
        /// </summary>
        public IEnumerable<CartDetailsDto> CartDetails { get; set; } = [];

        /// <summary>
        /// Gets the version number associated with the checkout header, used for tracking and managing updates or changes to the transaction data.
        /// </summary>
        public uint Version { get; init; }
    }
}