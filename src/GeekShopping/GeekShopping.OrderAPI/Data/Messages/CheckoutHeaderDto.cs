using GeekShopping.MessageBus.Classes;
using GeekShopping.OrderAPI.Data.DTOs;

namespace GeekShopping.OrderAPI.Data.Messages
{
    /// <summary>
    /// Represents the header details for a checkout process.
    /// This DTO contains essential data required for processing an order, such as user information, payment details, and cart summary.
    /// </summary>
    public class CheckoutHeaderDto : BaseMessage
    {
        /// <summary>
        /// Gets the unique identifier associated with the user, which is used to link the checkout process and related order information to a specific individual within
        /// the system.
        /// </summary>
        public required Guid UserId { get; init; }

        /// <summary>
        /// Gets the coupon code applied during the checkout process, which may provide discounts or promotional offers to the user.
        /// </summary>
        public string? CouponCode { get; init; }

        /// <summary>
        /// Represents the total amount to be paid for the current checkout process, aggregated from the cart items' prices and any additional charges or discounts applied.
        /// </summary>
        public decimal PurchaseAmount { get; init; }

        /// <summary>
        /// Gets or sets the total discount applied to the purchase amount during the checkout process, reflecting the value deducted based on promotions, coupons, or
        /// other discounts.
        /// </summary>
        public decimal DiscountTotal { get; init; }

        /// <summary>
        /// Gets or sets the first name of the user associated with the checkout process.
        /// This property is used to personalize the order experience and associate the transaction with the user's details.
        /// </summary>
        public string FirstName { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the user associated with the checkout process, providing additional identification information for the individual making
        /// the transaction.
        /// </summary>
        public string LastName { get; init; } = string.Empty;

        /// <summary>
        /// Represents the specific time when the purchase transaction occurred during the checkout process.
        /// This property records the exact moment the purchase was completed, aiding in order tracking, auditing, and historical analysis of sales data.
        /// </summary>
        public DateTime PurchaseTime { get; init; }

        /// <summary>
        /// Represents the date and time associated with the checkout process, typically indicating when the transaction or order was initiated.
        /// </summary>
        public DateTime PuchaseDate { get; init; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the phone number associated with the checkout process, providing a point of contact for the user.
        /// </summary>
        public string Phone { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address associated with the checkout process.
        /// This property captures the customer's email for communication purposes, such as sending order confirmations, updates, and other notifications.
        /// </summary>
        public string Email { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the credit or debit card number used for the checkout transaction.
        /// This property contains the primary account number (PAN) for processing payments.
        /// </summary>
        public string CardNumber { get; init; } = string.Empty;

        /// <summary>
        /// Represents the expiration month and year of the payment card used during the checkout process.
        /// This property is essential for validating and processing card payments.
        /// </summary>
        public string ExpiryMonthYear { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the card verification value (CVV) of the payment method used during the checkout process.
        /// This property ensures additional security in verifying card authenticity.
        /// </summary>
        public string Cvv { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of items in the shopping cart, representing the cumulative quantity of all products being purchased.
        /// </summary>
        public int CartTotalItems { get; set; }

        /// <summary>
        /// Represents the collection of detailed cart items included in the checkout process.
        /// Each item in the collection provides specific information about a product added to the cart, such as product details, quantity, and associated prices.
        /// </summary>
        public List<CartDetailDto> CartDetails { get; set; } = [];

        /// <summary>
        /// Gets the version number associated with the entity, which can be used for concurrency control or tracking changes within the system's data, ensuring data
        /// consistency and integrity.
        /// </summary>
        public uint Version { get; init; }
    }
}