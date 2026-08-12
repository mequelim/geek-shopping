namespace GeekShopping.CartAPI.Model
{
    /// <summary>
    /// Represents the header details of a shopping cart, including user information and associated discounts.
    /// </summary>
    public class CartHeader : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user associated with the shopping cart.
        /// </summary>
        public required Guid UserId { get; init; }

        /// <summary>
        /// Gets or sets the code for the discount coupon applied to the shopping cart.
        /// </summary>
        public string? CouponCode { get; set; }

        /// <summary>
        /// Gets or sets the version of the product entity.
        /// </summary>
        /// <remarks>
        /// This property is used to track the version or state of the product entity, often for concurrency control or data synchronization purposes.
        /// It is an unsigned integer that increments with each update to the entity.
        /// </remarks>
        public uint Version { get; init; }
    }
}