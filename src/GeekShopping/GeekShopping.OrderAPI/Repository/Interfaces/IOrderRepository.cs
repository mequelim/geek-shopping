using GeekShopping.OrderAPI.Model;

namespace GeekShopping.OrderAPI.Repository.Interfaces
{
    /// <summary>
    /// Provides access to methods for managing order data in the system.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Adds a new order to the system.
        /// </summary>
        /// <param name="header">The <c>OrderHeader</c> object containing details of the order to be added.</param>
        /// <returns>
        /// A <c>Task</c> that represents the asynchronous operation. The task result contains a boolean value indicating whether the order was successfully added.
        /// </returns>
        Task<bool> AddOrder(OrderHeader header);

        /// <summary>
        /// Updates the payment status of an order in the system.
        /// </summary>
        /// <param name="orderHeaderId">The unique identifier of the <c>OrderHeader</c> to update.</param>
        /// <param name="isPaid">A boolean value indicating the new payment status of the order.</param>
        /// <returns>
        /// A <c>Task</c> that represents the asynchronous operation. The task result contains a boolean value indicating whether the payment status update was successful.
        /// </returns>
        Task UpdateOrderPaymentStatus(Guid orderHeaderId, bool isPaid);
    }
}