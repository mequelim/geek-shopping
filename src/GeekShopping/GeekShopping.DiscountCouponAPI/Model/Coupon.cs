namespace GeekShopping.DiscountCouponAPI.Model
{
    /// <summary>
    /// Represents a product entity that defines essential properties of a product, including its name, price, optional discount, description,
    /// category, and image URL.
    /// This class inherits from the <see cref="BaseEntity"/> which provides a unique identifier for the product.
    /// </summary>
    public class Coupon
    {
        /// <summary>
        /// Gets or sets the unique identifier for an entity.
        /// </summary>
        /// <remarks>
        /// This property serves as the primary key for this entity.
        /// It is a unique identifier (GUID) that ensures uniqueness across the system.
        /// </remarks>
        public Guid Id { get; init; }

        /// <summary>
        /// Gets or sets the unique code representing the coupon.
        /// </summary>
        /// <remarks>
        /// This property is used to identify a specific discount coupon.
        /// It is a unique code that can be applied to qualify for discounts or offers.
        /// </remarks>
        public required string CouponCode { get; init; }

        /// <summary>
        /// Gets or sets the discount amount associated with the coupon.
        /// </summary>
        /// <remarks>
        /// This property specifies the monetary value of the discount that the coupon applies.
        /// It is expressed as a decimal and can be null if no discount is applicable.
        /// </remarks>
        public decimal DiscountAmount { get; init; }

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