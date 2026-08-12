namespace GeekShopping.CartAPI.Data.DTOs
{
    /// <summary>
    /// Represents a product with its essential properties.
    /// </summary>
    public class CouponDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for an entity.
        /// </summary>
        /// <remarks>
        /// This property serves as the primary key for this entity.
        /// It is a unique identifier (GUID) that ensures uniqueness across the system.
        /// </remarks>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the coupon code associated with a discount.
        /// </summary>
        /// <remarks>
        /// This property represents a unique code that can be applied to receive a discount.
        /// It is typically alphanumeric and defined by the system or administrators.
        /// </remarks>
        public string CouponCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the discount amount associated with the coupon.
        /// </summary>
        /// <remarks>
        /// This property specifies the monetary value of the discount that the coupon applies.
        /// It is expressed as a decimal and can be null if no discount is applicable.
        /// </remarks>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets the version of the product entity.
        /// </summary>
        /// <remarks>
        /// This property is used to track the version or state of the product entity, often for concurrency control or data synchronization purposes.
        /// It is an unsigned integer that increments with each update to the entity.
        /// </remarks>
        public uint Version { get; set; }
    }
}