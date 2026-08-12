namespace GeekShopping.DiscountCouponAPI.Shared.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a conflict occurs during execution.
    /// Commonly used to signal issues such as conflicting states, operations, or resource access disputes.
    /// </summary>
    public class ConflictException : Exception
    {
        /// <summary>
        /// Gets the name of the entity associated with the conflict.
        /// This property provides additional context about the resource or entity that caused the conflict, aiding in error tracking and resolution.
        /// </summary>
        public string? EntityName { get; }

        /// <summary>
        /// Represents an exception thrown when a conflict occurs during the execution of a program.
        /// This may happen when there are conflicting states, operations, or resource access that cannot be resolved.
        /// </summary>
        public ConflictException(string? message) : base(message) { }

        /// <summary>
        /// Defines an exception thrown when a conflict arises within an application.
        /// Typically, it indicates that an operation cannot proceed due to conflicting states or resource usage disputes.
        /// </summary>
        public ConflictException(string message, string? entityName) : base(message) => EntityName = entityName;
    }
}