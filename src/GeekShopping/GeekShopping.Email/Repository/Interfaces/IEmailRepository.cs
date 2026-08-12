using GeekShopping.Email.Data.DTOs;

namespace GeekShopping.Email.Repository.Interfaces
{
    /// <summary>
    /// Provides access to methods for managing order data in the system.
    /// </summary>
    public interface IEmailRepository
    {
        /// <summary>
        /// Logs the email associated with the update of a payment result.
        /// </summary>
        /// <param name="message">The DTO containing the order ID, email address, and payment status information.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task LogEmail(UpdatePaymentResultMessage message);
    }
}