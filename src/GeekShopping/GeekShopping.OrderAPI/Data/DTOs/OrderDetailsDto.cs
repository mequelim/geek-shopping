using GeekShopping.OrderAPI.Model;

namespace GeekShopping.OrderAPI.Data.DTOs
{
    /// <summary>
    /// Data transfer object representing the details of an order.
    /// </summary>
    /// <remarks>
    /// The <c>OrderDetailsDto</c> class provides a simplified way of transferring detailed information about an order between different layers or services.
    /// It includes properties such as the product, its quantity, and references to associated entities like the order header and cart header.
    /// </remarks>
    public class OrderDetailsDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the order detail.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// Gets or sets the unique identifier for the associated cart header.
        /// </summary>
        public Guid CartHeaderId { get; init; }

        /// <summary>
        /// Gets or sets the associated order header containing overarching details about the order.
        /// </summary>
        /// <remarks>
        /// The order header includes information such as user details, purchase dates, payment details, and other metadata critical to identifying and processing the order.
        /// </remarks>
        public OrderHeader? OrderHeader { get; init; }

        /// <summary>
        /// Gets or sets the unique identifier for the product associated with the order detail.
        /// </summary>
        public Guid ProductId { get; init; }

        /// <summary>
        /// Gets or sets the product associated with the order detail.
        /// </summary>
        /// <remarks>
        /// This property represents the product associated with a specific order detail, allowing access to detail about the product being purchased within an order.
        /// </remarks>
        public ProductDto? Product { get; init; }

        /// <summary>
        /// Gets or sets the quantity of the product associated with the order detail.
        /// </summary>
        /// <remarks>This property represents the number of units of a specific product included in the order.</remarks>
        public int Count { get; init; }

        /// <summary>
        /// Gets or sets the version number associated with the entity or data transfer object.
        /// </summary>
        /// <remarks>This property is typically used for concurrency control or tracking changes in the entity's state.</remarks>
        public uint Version { get; init; }
    }
}