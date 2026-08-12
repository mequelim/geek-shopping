using GeekShopping.CartAPI.Data.DTOs;

namespace GeekShopping.CartAPI.Repository.Interfaces
{
    /// <summary>
    /// Represents a repository interface for managing coupons.
    /// Provides methods for retrieving coupon details based on coupon codes.
    /// </summary>
    public interface ICouponRepository
    {
        /// <summary>
        /// Retrieves a coupon based on the provided coupon code.
        /// </summary>
        /// <param name="couponCode">The code of the coupon to retrieve.</param>
        /// <param name="token">The authentication token used for authorizing the request.</param>
        /// <returns>A <c>CouponDto</c> object if the coupon exists, or <c>null</c> if no coupon is found.</returns>
        Task<CouponDto?> GetCouponByCouponCode(string couponCode, string token);
    }
}