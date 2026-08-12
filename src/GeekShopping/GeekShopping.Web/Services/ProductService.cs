using GeekShopping.Web.Models;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Shared.Extensions;
using System.Net.Http.Headers;

namespace GeekShopping.Web.Services
{
    public class ProductService(HttpClient httpClient) : IProductService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private const string BasePath = "api/Products";

        // Methods:
        /// <summary>
        /// Retrieves a collection of all products by performing an asynchronous HTTP GET request.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains an enumerable collection of <see cref="ProductViewModel"/> objects representing the retrieved products.
        /// </returns>
        public async Task<IEnumerable<ProductViewModel>> GetAllProducts()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{BasePath}/GetAllProducts");
            return await response.ReadContextAs<List<ProductViewModel>>();
        }

        /// <summary>
        /// Retrieves a product by its unique identifier by performing an asynchronous HTTP GET request.
        /// </summary>
        /// <param name="id">The unique identifier of the product to be retrieved.</param>
        /// <param name="token">The access token used for authorization in the API request.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains an instance of <see cref="ProductViewModel"/> representing
        /// the retrieved product.
        /// </returns>
        /// <exception cref="HttpRequestException">
        /// Thrown when the HTTP request to the product API fails or returns a non-success status code.
        /// </exception>
        /// <exception cref="Exception">
        /// Thrown when there is a general exception during the operation or a connectivity issue with the product API.
        /// </exception>
        public async Task<ProductViewModel> GetProductById(Guid id, string? token)
        {
            try
            {
                if(id.Equals(Guid.Empty)) throw new HttpRequestException("Product ID is empty!");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync($"{BasePath}/GetProductById/{id}");

                if(!response.IsSuccessStatusCode) throw new HttpRequestException($"API Error: {response.StatusCode}.");

                return await response.ReadContextAs<ProductViewModel>();
            }
            catch(HttpRequestException exception)
            {
                await Console.Error.WriteLineAsync($"An exception occurred: {exception.Message}.");
                throw;
            }
            catch(Exception exception)
            {
                Console.WriteLine($"An exception occurred: {exception.Message}.");
                throw new Exception("Error to connect to Product API!");
            }
        }

        /// <summary>
        /// Creates a new product by performing an asynchronous HTTP POST request.
        /// </summary>
        /// <param name="productViewModel">The product model containing the details of the product to be created.</param>
        /// <param name="token">The authorization token to be included in the request header.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a <see cref="ProductViewModel"/> object representing the created
        /// product.
        /// </returns>
        /// <exception cref="HttpRequestException">Thrown when an error occurs while making the HTTP request to the product API.</exception>
        /// <exception cref="Exception">Thrown for general exceptions that may occur during the operation.</exception>
        public async Task<ProductViewModel> CreateProduct(ProductViewModel productViewModel, string? token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{BasePath}/CreateProduct", productViewModel);

                if(!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"API error: {errorContent}.");

                    throw new HttpRequestException($"Something went wrong calling the API: {response.StatusCode} - {errorContent}");
                }

                return await response.ReadContextAs<ProductViewModel>();
            }
            catch(HttpRequestException exception)
            {
                await Console.Error.WriteLineAsync($"An exception occurred: {exception.Message}.");
                throw;
            }
            catch(Exception exception)
            {
                Console.WriteLine($"An exception occurred: {exception.Message}.");
                throw new Exception("Error to connect to Product API!");
            }
        }

        /// <summary>
        /// Updates an existing product by sending an asynchronous HTTP PUT request to the Product API.
        /// </summary>
        /// <param name="productViewModel">The <see cref="ProductViewModel"/> object containing the updated product data.</param>
        /// <param name="token">The authentication token used to authorize the API request.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains the updated <see cref="ProductViewModel"/> object returned by the API.
        /// </returns>
        /// <exception cref="HttpRequestException">Thrown if the product ID is empty or if an HTTP request issue occurs.</exception>
        /// <exception cref="Exception">Thrown if a general error occurs during the operation.</exception>
        public async Task<ProductViewModel> UpdateProduct(ProductViewModel productViewModel, string? token)
        {
            try
            {
                if(productViewModel.Id.Equals(Guid.Empty)) throw new HttpRequestException("Product ID is empty before calling API!");

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.PutAsJsonAsync($"{BasePath}/UpdateProduct/{productViewModel.Id}", productViewModel);

                if(!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"API error: {errorContent}.");
                    throw new HttpRequestException($"Something went wrong calling the API: {response.StatusCode} - {errorContent}");
                }

                return await response.ReadContextAs<ProductViewModel>();
            }
            catch(HttpRequestException exception)
            {
                await Console.Error.WriteLineAsync($"An exception occurred: {exception.Message}.");
                throw;
            }
            catch(Exception exception)
            {
                Console.WriteLine($"An exception occurred: {exception.Message}");
                throw new Exception("Error to connect to Product API!");
            }
        }

        /// <summary>
        /// Deletes a product identified by the specified ID and version by performing an asynchronous HTTP DELETE request.
        /// </summary>
        /// <param name="id">The unique identifier of the product to be deleted.</param>
        /// <param name="version">The version of the product for concurrency control.</param>
        /// <param name="token">The Bearer token used for authorization.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a boolean value indicating whether the product was
        /// successfully deleted.
        /// </returns>
        /// <exception cref="HttpRequestException">Thrown when the HTTP request to the API fails.</exception>
        /// <exception cref="Exception">Thrown when an unexpected error occurs during the operation.</exception>
        public async Task<bool> DeleteProduct(Guid id, uint version, string? token)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient
                    .DeleteAsync($"{BasePath}/DeleteProduct/{id}?version={version}");

                if(!response.IsSuccessStatusCode) throw new HttpRequestException($"Something went wrong calling the API: {response}!");

                return await response.ReadContextAs<bool>();
            }
            catch(HttpRequestException exception)
            {
                await Console.Error.WriteLineAsync($"An exception occurred: {exception.Message}.");
                throw;
            }
            catch(Exception exception)
            {
                Console.WriteLine($"An exception occurred: {exception.Message}");
                throw new Exception("Error to connect to Product API!");
            }
        }
    }
}