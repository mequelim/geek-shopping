using GeekShopping.CartAPI.Data.DTOs;

namespace GeekShopping.CartAPI.Repository.Interfaces
{
    /// <summary>
    /// Defines the contract for a repository that manages shopping cart operations, including retrieval, updates, and coupon application.
    /// </summary>
    public interface ICartRepository
    {
        /// <summary>
        /// Retrieves the shopping cart associated with the specified user identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user whose cart is to be retrieved.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the 
        /// <see cref="GeekShopping.CartAPI.Data.DTOs.CartDto"/> object representing the user's shopping cart.
        /// </returns>
        Task<CartDto> FindCartByUserId(Guid id);

        /// <summary>
        /// Saves a new shopping cart or updates an existing one based on the provided cart data.
        /// </summary>
        /// <param name="cartDto">The data transfer object representing the shopping cart, including header and item details.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains the
        /// <see cref="GeekShopping.CartAPI.Data.DTOs.CartDto"/> object representing the updated or newly created shopping cart.
        /// </returns>
        Task<CartDto> SaveOrUpdateCart(CartDto cartDto);

        /// <summary>
        /// Removes an item from the shopping cart based on the specified cart details identifier.
        /// </summary>
        /// <param name="cartDetailsId">
        /// The unique identifier of the cart item to be removed.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is a boolean value indicating whether the item was successfully removed from the cart.
        /// </returns>
        Task<bool> RemoveFromCart(Guid cartDetailsId);

        /// <summary>
        /// Clears the shopping cart associated with the specified user identifier.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose cart is to be cleared.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is a boolean value indicating whether the cart was successfully cleared.
        /// </returns>
        Task<bool> ClearCart(Guid userId);

        /// <summary>
        /// Applies a coupon to the shopping cart of the specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart the coupon will be applied to.</param>
        /// <param name="couponCode">The code of the coupon to be applied.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is a boolean value indicating whether the coupon was successfully applied to the user's cart.
        /// </returns>
        Task<bool> ApplyCoupon(Guid userId, string? couponCode);

        /// <summary>
        /// Removes a coupon associated with the specified user's shopping cart.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose cart's coupon is to be removed.
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is a boolean value indicating whether the coupon was successfully removed from the user's cart.
        /// </returns>
        Task<bool> RemoveCoupon(Guid userId);
    }
}