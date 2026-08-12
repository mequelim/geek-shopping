using GeekShopping.CartAPI.Model;

namespace GeekShopping.CartAPI.Data.DTOs
{
    /// <summary>
    /// Represents the details of items in a shopping cart, including item-specific information such as quantity and associated product.
    /// </summary>
    public class CartDetailsDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the cart details.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Gets or sets the unique identifier of the associated cart header.
        /// </summary>
        public Guid CartHeaderId { get; init; }

        /// <summary>
        /// Gets the associated cart header.
        /// </summary>
        public CartHeader? CartHeader { get; init; }

        /// <summary>
        /// Gets the unique identifier of the product associated with the cart details.
        /// </summary>
        public Guid ProductId { get; init; }

        /// <summary>
        /// Gets the product associated with the cart detail.
        /// </summary>
        public Product? Product { get; init; }

        /// <summary>
        /// Gets the quantity of a specific product within the shopping cart.
        /// </summary>
        public int Count { get; init; }

        /// <summary>
        /// Gets the version of the entity used for concurrency control.
        /// </summary>
        public uint Version { get; init; }
    }
}