namespace GeekShopping.OrderAPI.Model
{
    /// <summary>
    /// Represents the header of an order, containing key properties related to the order's user, coupon code, and versioning.
    /// </summary>
    /// <remarks>
    /// The <c>OrderHeader</c> class extends the <c>BaseEntity</c> class and serves as the primary description of an order.
    /// It includes information about the user associated with the order, any applied coupon codes, and a version property for concurrency control or tracking changes.
    /// </remarks>
    public class OrderHeader : BaseEntity
    {
        /// <summary>
        /// Gets the unique identifier of the user associated with the order.
        /// </summary>
        public required Guid UserId { get; init; }

        /// <summary>
        /// Gets the coupon code applied to the order, if any.
        /// </summary>
        public string? CouponCode { get; init; }

        /// <summary>
        /// Gets or sets the total monetary value of the order before discounts and adjustments.
        /// </summary>
        /// <remarks>
        /// The <c>PurchaseAmount</c> property represents the sum of all item prices in the order.
        /// It reflects the original cost prior to applying any discounts, taxes, or additional charges.
        /// </remarks>
        public decimal PurchaseAmount { get; set; }

        /// <summary>
        /// Gets or sets the total discount applied to the order.
        /// </summary>
        /// <remarks>
        /// This property represents the cumulative value of all discounts applied to the order.
        /// It is used to calculate the final purchase amount after discounts have been accounted for.
        /// </remarks>
        public decimal DiscountTotal { get; set; }

        /// <summary>
        /// Gets or sets the first name of the individual associated with the order.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the last name of the individual associated with the order.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets the date and time when the purchase was made.
        /// </summary>
        public DateTime PurchaseDate { get; init; }

        /// <summary>
        /// Gets the date and time when the purchase was made.
        /// </summary>
        public DateTime PurchaseTime { get; init; }

        /// <summary>
        /// Gets or sets the phone number associated with the order.
        /// </summary>
        public string Phone { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address associated with the order.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the card number used for the transaction associated with the order.
        /// </summary>
        public string CardNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the expiration date of the card in the format MM/YY.
        /// </summary>
        public string ExpiryMonthYear { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the card verification value (CVV) associated with the payment method.
        /// </summary>
        public string Cvv { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the total number of items included in the order.
        /// </summary>
        public int OrderTotalItems { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the order has been paid.
        /// </summary>
        /// <remarks>
        /// This property reflects whether the full payment for the order has been successfully processed.
        /// It is used to determine the payment status of the associated order.
        /// </remarks>
        public bool IsPaid { get; set; }

        /// <summary>
        /// Represents a collection of order details associated with an order header.
        /// Each item in the collection provides detailed information about individual products included in the order.
        /// </summary>
        public List<OrderDetail> OrderDetails { get; init; } = [];

        /// <summary>
        /// Gets the version number associated with the checkout header, used for tracking and managing updates or changes to the transaction data.
        /// </summary>
        public uint Version { get; init; }
    }
}