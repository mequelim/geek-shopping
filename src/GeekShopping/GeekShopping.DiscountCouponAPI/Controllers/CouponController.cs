using GeekShopping.DiscountCouponAPI.Data.DTOs;
using GeekShopping.DiscountCouponAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.DiscountCouponAPI.Controllers
{
    /// <summary>
    /// Controller responsible for managing coupon-related operations.
    /// Provides endpoints for retrieving coupon details by their code.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController(ICouponRepository couponRepository) : ControllerBase
    {
        private readonly ICouponRepository _couponRepository = couponRepository
                                                               ?? throw new ArgumentNullException(nameof(couponRepository));

        //? GET:
        /// <summary>
        /// Retrieves a coupon based on the provided coupon code.
        /// </summary>
        /// <param name="couponCode">The unique code of the coupon to retrieve.</param>
        /// <returns>
        /// An <see cref="ActionResult{T}"/> containing a <see cref="CouponDto"/> object representing the coupon details if the coupon code exists; otherwise, an appropriate HTTP response.
        /// </returns>
        [HttpGet("GetCouponByCouponCode/{couponCode}")]
        [Authorize]
        public async Task<ActionResult<CouponDto>> GetCouponByCouponCode(string couponCode)
        {
            CouponDto? coupon = await _couponRepository.GetCouponByCouponCode(couponCode);

            if(coupon is null) return NotFound();

            return Ok(coupon);
        }
    }
}