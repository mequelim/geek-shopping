namespace GeekShopping.OrderAPI.Model
{
    /// <summary>
    /// Represents the base class for all entities in the system.
    /// </summary>
    /// <remarks>
    /// The <c>BaseEntity</c> class provides a foundation for other entities by defining shared properties and behaviors.
    /// In particular, it includes a primary key property to uniquely identify each entity.
    /// This class is intended to be inherited by other entity classes within the application.
    /// </remarks>
    public class BaseEntity
    {
        /// <summary>
        /// Gets or sets the unique identifier for an entity.
        /// </summary>
        /// <remarks>
        /// This property serves as the primary key for entities that inherit from the <see cref="BaseEntity"/> class.
        /// It is a globally unique identifier (GUID) that ensures uniqueness across the system.
        /// </remarks>
        public Guid Id { get; set; }
    }
}