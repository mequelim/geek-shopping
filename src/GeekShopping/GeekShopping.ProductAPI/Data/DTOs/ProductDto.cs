namespace GeekShopping.ProductAPI.Data.DTOs
{
    /// <summary>
    /// Represents the data transfer object for a product.
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for a product.
        /// </summary>
        public Guid? Id { get; init; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        public required string Name { get; init; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public required decimal Price { get; init; }

        /// <summary>
        /// Gets or sets the discount applicable to the product, if any.
        /// </summary>
        public decimal? Discount { get; init; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Gets or sets the category of the product.
        /// </summary>
        public required string Category { get; init; }

        /// <summary>
        /// Gets or sets the URL of the product image.
        /// </summary>
        public required string ImageUrl { get; init; }

        /// <summary>
        /// Gets or sets the version of the product, which can be used to track changes or updates to the product's information.
        /// </summary>
        public uint Version { get; init; }
    }
}