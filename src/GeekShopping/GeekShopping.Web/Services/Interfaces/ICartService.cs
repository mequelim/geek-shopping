using GeekShopping.Web.Models;

namespace GeekShopping.Web.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartViewModel> FindCartByUserId(Guid userId, string token);
        Task<CartViewModel> AddItemToCart(CartViewModel cart, string token);
        Task<CartViewModel> UpdateCart(CartViewModel cart, string token);
        Task<bool> DeleteItemFromCart(Guid cartId, string token);
        // Task<bool> ClearCart(Guid userId, string token);
        Task<bool> ApplyCoupon(CartViewModel cart, string token);
        Task<bool> RemoveCoupon(Guid userId, string token);
        Task<object> Checkout(CartHeaderViewModel cartHeader, string token);
    }
}