using GeekShopping.DiscountCouponAPI.Data.DTOs;

namespace GeekShopping.DiscountCouponAPI.Repository.Interfaces
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
        /// <param name="couponCode">The unique code of the coupon to retrieve.</param>
        /// <returns>
        /// A task that represents the asynchronous operation.
        /// The task result contains the <see cref="CouponDto"/> object representing the coupon details if the coupon code exists; otherwise, null.
        /// </returns>
        Task<CouponDto?> GetCouponByCouponCode(string couponCode);
    }
}