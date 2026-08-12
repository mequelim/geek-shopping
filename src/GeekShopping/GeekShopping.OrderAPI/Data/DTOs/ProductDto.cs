namespace GeekShopping.OrderAPI.Data.DTOs
{
    /// <summary>
    /// Represents a product with its essential properties.
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Gets the unique identifier of the product.
        /// </summary>
        public Guid? Id { get; init; }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets the price of the product.
        /// </summary>
        public required decimal Price { get; init; }

        /// <summary>
        /// Gets the optional discount applied to the product.
        /// </summary>
        public decimal? Discount { get; init; }

        /// <summary>
        /// Gets the description of the product.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Gets the category of the product.
        /// </summary>
        public required string Category { get; init; }

        /// <summary>
        /// Gets the URL of the image associated with the product.
        /// </summary>
        public required string ImageUrl { get; init; }

        /// <summary>
        /// Gets the version of the entity, used for concurrency control.
        /// </summary>
        public uint Version { get; init; }
    }
}