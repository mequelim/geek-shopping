using GeekShopping.OrderAPI.Infrastructure.Database;
using GeekShopping.OrderAPI.Model;
using GeekShopping.OrderAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Repository
{
    /// <summary>
    /// Provides functionality to interact with order data in the database.
    /// </summary>
    public class OrderRepository(DbContextOptions<AppDbContext> databaseContext) : IOrderRepository
    {
        private readonly DbContextOptions<AppDbContext> _databaseContext = databaseContext
                                                                           ?? throw new ArgumentNullException(nameof(databaseContext));

        // Methods:
        /// <summary>
        /// Adds a new order to the database.
        /// </summary>
        /// <param name="header">The <see cref="OrderHeader"/> instance containing the details of the order to be added.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the order was successfully added.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="header"/> parameter is null.</exception>
        /// <exception cref="DbUpdateException">Thrown when there is an error updating the database.</exception>
        public async Task<bool> AddOrder(OrderHeader header)
        {
            await using AppDbContext database = new(_databaseContext);
            database.OrderHeaders.Add(header);
            await database.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Updates the payment status of an order identified by the provided order header ID.
        /// </summary>
        /// <param name="orderHeaderId">The unique identifier of the order header whose payment status is to be updated.</param>
        /// <param name="isPaid">A boolean value indicating whether the order should be marked as paid.</param>
        /// <returns>A task that represents the asynchronous operation of updating the payment status.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="orderHeaderId"/> is empty or uninitialized.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the specified order header is not found in the database.</exception>
        /// <exception cref="DbUpdateException">Thrown when there is an error saving changes to the database.</exception>
        public async Task UpdateOrderPaymentStatus(Guid orderHeaderId, bool isPaid)
        {
            await using AppDbContext database = new(_databaseContext);
            OrderHeader? header = await database.OrderHeaders
                .FirstOrDefaultAsync((order) => order.Id.Equals(orderHeaderId));

            if(header is not null)
            {
                header.IsPaid = isPaid;
                await database.SaveChangesAsync();
            }
        }
    }
}