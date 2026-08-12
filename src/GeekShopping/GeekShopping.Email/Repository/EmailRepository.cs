using GeekShopping.Email.Data.DTOs;
using GeekShopping.Email.Infrastructure.Database;
using GeekShopping.Email.Model;
using GeekShopping.Email.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.Email.Repository
{
    /// <summary>
    /// Provides methods for managing email-related operations in the application.
    /// </summary>
    public class EmailRepository(DbContextOptions<AppDbContext> databaseContext) : IEmailRepository
    {
        private readonly DbContextOptions<AppDbContext> _databaseContext = databaseContext
                                                                           ?? throw new ArgumentNullException(nameof(databaseContext));

        // Methods:
        /// <summary>
        /// Logs email information to the database, including email content, log information, and the timestamp when the email was sent.
        /// </summary>
        /// <param name="message">The data transfer object containing the email content, order identifier, and payment result status associated with the email log.</param>
        /// <returns>A task that represents the asynchronous operation for logging the email data.</returns>
        public async Task LogEmail(UpdatePaymentResultMessage message)
        {
            EmailLog email = new()
            {
                Email = message.Email,
                SentDate = DateTime.Now.ToUniversalTime(),
                Log = $"Order {message.OrderId} has been created successfully!"
            };

            await using AppDbContext database = new(_databaseContext);
            database.Email.Add(email);
            await database.SaveChangesAsync();
        }
    }
}