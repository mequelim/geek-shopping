namespace GeekShopping.OrderAPI.Model
{
    /// <summary>
    /// Represents the details of an order, including references to the associated cart header, product, and the quantity of the product in the order.
    /// </summary>
    /// <remarks>
    /// The <c>OrderDetail</c> class extends <c>BaseEntity</c> and serves as a model for tracking individual product details within an order.
    /// It contains references to the <c>CartHeader</c> and <c>Product</c> entities it is associated with as well as additional fields for tracking the product quantity and versioning.
    /// </remarks>
    public sealed class OrderDetail : BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier of the associated cart header.
        /// This property represents the foreign key relationship to the cart header entity, allowing the linking of cart details to their respective session and context within the shopping cart system.
        /// </summary>
        public Guid OrderHeaderId { get; set; }

        /// <summary>
        /// Gets the associated cart header, which contains metadata and contextual information about the shopping cart.
        /// This property establishes the relationship between the current cart details and the overarching shopping cart header, enabling a cohesive link between the items in the cart and their
        /// session-specific context.
        /// </summary>
        public OrderHeader? OrderHeader { get; init; }

        /// <summary>
        /// Gets the unique identifier of the product associated with the cart details.
        /// This property links a specific product to the shopping cart system, enabling the association and tracking of product-level information for items added to the cart.
        /// </summary>
        public Guid ProductId { get; init; }

        /// <summary>
        /// Gets or sets the name of the product associated with the order.
        /// This property represents the descriptive title or identifier of the product included in the order details, allowing for clear identification of the item.
        /// </summary>
        public required string ProductName { get; init; }

        /// <summary>
        /// Gets or sets the unit price of the product in the order.
        /// This property represents the monetary value per unit of the product and is used for calculating the total cost of the order based on the quantity specified.
        /// </summary>
        public decimal Price { get; set; }

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