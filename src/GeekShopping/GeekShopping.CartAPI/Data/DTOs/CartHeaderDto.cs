namespace GeekShopping.CartAPI.Data.DTOs
{
    /// <summary>
    /// Represents the header details of a shopping cart, including user information and associated discounts.
    /// </summary>
    public class CartHeaderDto
    {
        /// <summary>
        /// Gets the unique identifier of the cart header.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Gets the unique identifier for the user associated with the shopping cart.
        /// </summary>
        public required Guid UserId { get; init; }

        /// <summary>
        /// Gets the discount coupon code applied to the shopping cart.
        /// </summary>
        public string? CouponCode { get; init; }

        /// <summary>
        /// Gets the version of the entity, used for concurrency control.
        /// </summary>
        public uint Version { get; init; }
    }
}