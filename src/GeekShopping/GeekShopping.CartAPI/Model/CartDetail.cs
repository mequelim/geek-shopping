namespace GeekShopping.CartAPI.Model
{
    /// <summary>
    /// Represents the details of a shopping cart item, including linkage to a specific cart header and product.
    /// </summary>
    public sealed class CartDetail : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the associated cart header.
        /// This property represents the foreign key relationship to the cart header entity, allowing the linking of cart details to their respective session and context within the shopping cart system.
        /// </summary>
        public Guid CartHeaderId { get; set; }

        /// <summary>
        /// Gets the associated cart header, which contains metadata and contextual information about the shopping cart.
        /// This property establishes the relationship between the current cart details and the overarching shopping cart header, enabling a cohesive link between the items in the cart and their session-specific context.
        /// </summary>
        public CartHeader? CartHeader { get; init; }

        /// <summary>
        /// Gets the unique identifier of the product associated with the cart details.
        /// This property links a specific product to the shopping cart system, enabling the association and tracking of product-level information for items added to the cart.
        /// </summary>
        public Guid ProductId { get; init; }

        /// <summary>
        /// Represents the product associated with a specific cart detail.
        /// This property links the cart item to its corresponding product, including essential information such as name, price, category, and additional attributes defined in the <see cref="Product"/> entity.
        /// </summary>
        public Product? Product { get; set; }

        /// <summary>
        /// Gets the quantity of a specific product within the shopping cart.
        /// This property represents the number of units of the associated product included in the cart, allowing for accurate tracking and calculation of totals during checkout.
        /// </summary>
        public int Count { get; set; }

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