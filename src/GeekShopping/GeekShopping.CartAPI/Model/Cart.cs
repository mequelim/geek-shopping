namespace GeekShopping.CartAPI.Model
{
    /// <summary>
    /// Represents a shopping cart containing a header and a collection of cart item details.
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// Gets the header of the shopping cart, containing user-specific information and coupon details associated with the cart.
        /// </summary>
        public CartHeader? CartHeader { get; init; }

        /// <summary>
        /// Represents a collection of shopping cart item details, linking products and quantities to a specific cart header for a comprehensive view of the cart's contents.
        /// </summary>
        public IEnumerable<CartDetail>? CartDetails { get; init; }
    }
}