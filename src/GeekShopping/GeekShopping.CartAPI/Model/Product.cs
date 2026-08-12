namespace GeekShopping.CartAPI.Model
{
    /// <summary>
    /// Represents a product entity that defines essential properties of a product, including its name, price, optional discount, description,
    /// category, and image URL.
    /// This class inherits from the <see cref="BaseEntity"/> which provides a unique identifier for the product.
    /// </summary>
    public class Product
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
        /// Gets the name of the product.
        /// This is a required property and represents the unique name assigned to the product.
        /// The maximum allowable length for this property is 300 characters.
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Gets the price of the product.
        /// This property is required and represents the monetary value assigned to the product.
        /// The price is stored with precision of up to 7 digits, including 2 decimal places.
        /// </summary>
        public decimal Price { get; init; }

        /// <summary>
        /// Gets the optional discount applied to the product.
        /// The value represents a percentage discount and is stored as a decimal value between 0 and 1.
        /// If no discount is specified, the default value is 0.
        /// This property is nullable, allowing products to have no discount applied.
        /// </summary>
        public decimal? Discount { get; init; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// This property provides additional details about the product, enhancing the information available for end users or systems.
        /// The maximum allowable length for this property is 500 characters.
        /// This property is optional.
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Gets the category of the product.
        /// This property is required and specifies the classification or type of the product.
        /// The maximum allowable length for this property is 50 characters.
        /// </summary>
        public string? Category { get; init; }

        /// <summary>
        /// Gets the URL of the image associated with the product.
        /// This is a required property and is used to reference the visual representation of the product in external storage or CDNs.
        /// The maximum allowable length for this property is 300 characters.
        /// </summary>
        public string? ImageUrl { get; init; }

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