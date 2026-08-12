using AutoMapper;
using GeekShopping.CartAPI.Data.DTOs;
using GeekShopping.CartAPI.Infrastructure.Database;
using GeekShopping.CartAPI.Model;
using GeekShopping.CartAPI.Repository.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GeekShopping.CartAPI.Repository
{
    /// <summary>
    /// Provides an implementation for managing coupon data operations within the application.
    /// </summary>
    /// <remarks>
    /// This repository interacts with the database through the <see cref="AppDbContext"/> and uses <see cref="IMapper"/> for mapping data entities to Data Transfer Objects (DTOs).
    /// It implements the <see cref="ICouponRepository"/> interface to ensure consistency in coupon data access patterns.
    /// </remarks>
    public class CouponRepository(HttpClient httpClient) : ICouponRepository
    {
        // Method:
        /// <summary>
        /// Retrieves a coupon by its coupon code.
        /// </summary>
        /// <param name="couponCode">The unique code of the coupon to be retrieved.</param>
        /// <param name="token">The authentication token required to access the API.</param>
        /// <returns>A <see cref="CouponDto"/> object representing the coupon details if found, or null if the coupon does not exist.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the response content cannot be deserialized into a <see cref="CouponDto"/> object.</exception>
        public async Task<CouponDto?> GetCouponByCouponCode(string couponCode, string token)
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await httpClient.GetAsync($"api/Coupon/{couponCode}");
            string content = await response.Content.ReadAsStringAsync();

            if(!response.StatusCode.Equals(HttpStatusCode.OK)) return new CouponDto();

            return JsonSerializer.Deserialize<CouponDto>(
                content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? throw new InvalidOperationException();
        }
    }
}