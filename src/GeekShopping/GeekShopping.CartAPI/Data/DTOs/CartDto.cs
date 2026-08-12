namespace GeekShopping.CartAPI.Data.DTOs
{
    /// <summary>
    /// Represents a shopping cart containing a header and a collection of cart item details.
    /// </summary>
    public class CartDto
    {
        /// <summary>
        /// Gets the header of the shopping cart.
        /// </summary>
        public required CartHeaderDto CartHeader { get; init; }

        /// <summary>
        /// Gets the collection of shopping cart item details.
        /// </summary>
        public required IEnumerable<CartDetailsDto> CartDetails { get; init; }

        /// <summary>
        /// Gets the version of the entity used for concurrency control.
        /// </summary>
        public uint Version { get; init; }
    }
}