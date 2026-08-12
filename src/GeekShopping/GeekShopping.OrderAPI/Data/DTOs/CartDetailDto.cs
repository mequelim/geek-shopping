namespace GeekShopping.OrderAPI.Data.DTOs
{
    /// <summary>
    /// Represents the details of a shopping cart item, including its association with a cart header and the product information.
    /// </summary>
    public class CartDetailDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the cart detail entry.
        /// This identifier uniquely represents a specific item in the shopping cart.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier for the associated cart header.
        /// This property links the cart detail to a specific cart header, representing the broader context of the shopping cart transaction.
        /// </summary>
        public Guid CartHeaderId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the product associated with the cart detail.
        /// This property links the cart detail to a specific product in the system.
        /// </summary>
        public Guid ProductId { get; set; }

        /// <summary>
        /// Gets or sets the product details associated with the cart item.
        /// This property contains information about the product being added to the shopping cart.
        /// </summary>
        public virtual ProductDto? Product { get; set; }

        /// <summary>
        /// Gets or sets the quantity of the product in the shopping cart.
        /// This value represents how many units of the product are included in the cart.
        /// </summary>
        public int Count { get; set; }
    }
}