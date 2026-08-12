using AutoMapper;
using GeekShopping.DiscountCouponAPI.Data.DTOs;
using GeekShopping.DiscountCouponAPI.Infrastructure.Database;
using GeekShopping.DiscountCouponAPI.Model;
using GeekShopping.DiscountCouponAPI.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.DiscountCouponAPI.Repository
{
    /// <summary>
    /// Provides an implementation for managing coupon data operations within the application.
    /// </summary>
    /// <remarks>
    /// This repository interacts with the database through the <see cref="AppDbContext"/> and uses <see cref="IMapper"/> for mapping data entities to Data Transfer Objects (DTOs).
    /// It implements the <see cref="ICouponRepository"/> interface to ensure consistency in coupon data access patterns.
    /// </remarks>
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _databaseContext;
        private readonly IMapper _mapper;

        // Constructor:
        /// <summary>
        /// Repository responsible for managing coupon data operations.
        /// Provides methods to handle coupon-related queries and interactions with the database context.
        /// </summary>
        /// <remarks>
        /// This class uses an instance of <see cref="AppDbContext"/> for database operations and <see cref="IMapper"/> for mapping data between entities and DTOs.
        /// Implements the <see cref="ICouponRepository"/> interface.
        /// </remarks>
        public CouponRepository(AppDbContext databaseContext, IMapper mapper)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        // Method:
        /// <summary>
        /// Retrieves a coupon from the database based on the provided coupon code.
        /// </summary>
        /// <param name="couponCode">The unique code identifying the coupon to be retrieved.</param>
        /// <returns>A <see cref="CouponDto"/> object containing the mapped coupon details if found, or null if no matching record is available.</returns>
        public async Task<CouponDto?> GetCouponByCouponCode(string couponCode)
        {
            Coupon? coupon = await _databaseContext.Coupons.FirstOrDefaultAsync((c) => c.CouponCode.Equals(couponCode));

            return _mapper.Map<CouponDto>(coupon);
        }
    }
}