using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Shared.Extensions;
using System.Net;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class CouponService(HttpClient httpClient) : ICouponService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private const string BasePath = "api/Coupon";

        // Methods:
        /// <summary>
        /// Retrieves the coupon details associated with the specified coupon code from the remote service.
        /// </summary>
        /// <param name="couponCode">The unique code identifying the coupon to retrieve. Cannot be null or empty.</param>
        /// <param name="token">The bearer token used to authorize the request. Must be a valid authentication token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a CouponViewModel with the details of the requested coupon.</returns>
        /// <exception cref="HttpRequestException">Thrown if the coupon code is null or empty, or if the HTTP request to the remote service fails or returns a non-success status code.</exception>
        public async Task<CouponViewModel?> GetCoupon(string? couponCode, string token)
        {
            if(string.IsNullOrEmpty(couponCode)) throw new HttpRequestException("Coupon code is empty!");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.GetAsync($"{BasePath}/GetCouponByCouponCode/{couponCode}");

            if(response.StatusCode.Equals(HttpStatusCode.NotFound)) return null;
            if(!response.IsSuccessStatusCode) throw new HttpRequestException($"API Error: {response.StatusCode}.");

            return await response.ReadContextAs<CouponViewModel>();
        }
    }
}