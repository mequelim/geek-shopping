using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Shared.Extensions;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class CartService(HttpClient httpClient) : ICartService
    {
        private readonly HttpClient _httpClient = httpClient
                                                  ?? throw new ArgumentNullException(nameof(httpClient));
        private const string BasePath = "api/Cart";

        // Methods:
        /// <summary>
        /// Retrieves the cart details for a specific user based on the provided user ID.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart is to be retrieved.</param>
        /// <param name="token">The authorization token used for API access.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an instance of <see cref="CartViewModel"/> with the cart details for the specified user.</returns>
        public async Task<CartViewModel> FindCartByUserId(Guid userId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.GetAsync($"{BasePath}/FindCartByUserId/{userId}");

            return await response.ReadContextAs<CartViewModel>();
        }

        /// <summary>
        /// Adds a new item to the cart based on the provided cart details.
        /// </summary>
        /// <param name="cart">The cart details containing the item to be added.</param>
        /// <param name="token">The authorization token used for API access.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an instance of <see cref="CartViewModel"/> with the updated cart after adding the item.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API call is unsuccessful or encounters an error.</exception>
        public async Task<CartViewModel> AddItemToCart(CartViewModel cart, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BasePath}/AddItemToCart", cart);

            if(!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($">>> API error: {errorContent}.");
                throw new HttpRequestException($"Something went wrong calling the API: {response.StatusCode} - {errorContent}");
            }

            return await response.ReadContextAs<CartViewModel>();
        }

        /// <summary>
        /// Updates the contents of an existing cart based on the provided cart details.
        /// </summary>
        /// <param name="cart">An instance of <see cref="CartViewModel"/> containing the updated details of the cart.</param>
        /// <param name="token">The authorization token used for API access.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains an instance of <see cref="CartViewModel"/> reflecting the changes made to the cart.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API call fails or an error response is returned.</exception>
        public async Task<CartViewModel> UpdateCart(CartViewModel cart, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.PatchAsJsonAsync($"{BasePath}/UpdateCart", cart);

            if(!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($">>> API error: {errorContent}.");
                throw new HttpRequestException($"Something went wrong calling the API: {response.StatusCode} - {errorContent}");
            }

            return await response.ReadContextAs<CartViewModel>();
        }

        /// <summary>
        /// Deletes a specific item from the cart using the provided cart ID.
        /// </summary>
        /// <param name="cartId">The unique identifier of the cart item to be deleted.</param>
        /// <param name="token">The authorization token used for API access.</param>
        /// <returns>A task representing the asynchronous operation. The task result indicates whether the item was successfully deleted.</returns>
        /// <exception cref="HttpRequestException">Thrown if the API call fails or returns an unsuccessful status code.</exception>
        public async Task<bool> DeleteItemFromCart(Guid cartId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BasePath}/RemoveFromCart/{cartId}");

            if(!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($">>> API error: {errorContent}.");
                throw new HttpRequestException($"Something went wrong calling the API: {response.StatusCode} - {errorContent}");
            }

            return await response.ReadContextAs<bool>();
        }

        /// <summary>
        /// Applies a coupon to the specified cart based on the provided coupon code.
        /// </summary>
        /// <param name="cart">The cart to which the coupon is to be applied, including cart details and purchase amount.</param>
        /// <param name="token">The authorization token required for API access.</param>
        /// <returns>A task representing the asynchronous operation. The task result indicates whether the coupon was successfully applied.</returns>
        /// <exception cref="HttpRequestException">Thrown when the API call fails or returns an error status code.</exception>
        public async Task<bool> ApplyCoupon(CartViewModel cart, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                cart.CartHeader.UserId,
                cart.CartHeader.CouponCode
            };

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BasePath}/ApplyCoupon", payload);

            if(!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($">>> API error: {errorContent}.");
                throw new HttpRequestException($"Something went wrong calling the API: {response.StatusCode} - {errorContent}");
            }

            return await response.ReadContextAs<bool>();
        }

        /// <summary>
        /// Removes a previously applied coupon from the cart for a specified user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose coupon is to be removed from the cart.</param>
        /// <param name="token">The authorization token used for API access.</param>
        /// <returns>A task representing the asynchronous operation. The task result indicates whether the coupon was successfully removed.</returns>
        /// <exception cref="HttpRequestException">Thrown when there is an error in the HTTP request or the API response indicates failure.</exception>
        public async Task<bool> RemoveCoupon(Guid userId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await _httpClient.DeleteAsync($"{BasePath}/RemoveCoupon/{userId}");

            if(!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($">>> API error: {errorContent}.");
                throw new HttpRequestException($"Something went wrong calling the API: {response.StatusCode} - {errorContent}");
            }

            return await response.ReadContextAs<bool>();
        }

        /// <summary>
        /// Processes the checkout for the cart associated with the provided cart header data.
        /// </summary>
        /// <param name="model">The model containing cart header information, such as user details, purchase amount, and discount details.</param>
        /// <param name="token">The authentication token required to authorize the checkout process.</param>
        /// <returns>A task representing the asynchronous operation. The task result is an object that contains the result of the checkout process. If the coupon price
        /// has changed, a string message indicating this is returned; otherwise, updated cart header information is returned.</returns>
        /// <exception cref="HttpRequestException">Thrown when there is an issue with the HTTP request to the API during the checkout process.</exception>
        public async Task<object> Checkout(CartHeaderViewModel model, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BasePath}/Checkout", model);

            if(!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($">>> API error: {errorContent}.");
                throw new HttpRequestException($"Something went wrong calling the API: {response.StatusCode} - {errorContent}");
            }
            else if(response.StatusCode.ToString().Equals("PreconditionFailed"))
            {
                return "Coupon price has changed. Please, confirm it!";
            }

            return await response.ReadContextAs<CartHeaderViewModel>();
        }
    }
}