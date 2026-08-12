namespace GeekShopping.CartAPI.Model
{
    /// <summary>
    /// Represents the base class for all entities in the application, providing a common framework for entity definitions.
    /// </summary>
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