using System.Net.Http.Headers;
using System.Text.Json;

namespace GeekShopping.Web.Shared.Extensions
{
    public static class HttpClientExtensions
    {
        private static readonly MediaTypeHeaderValue ContentType = new("application/json");

        /// <summary>
        /// Reads the content of an HTTP response and deserializes it into an instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type into which the HTTP response content should be deserialized.</typeparam>
        /// <param name="response">The HTTP response message containing the content to be deserialized.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the deserialized object of type <typeparamref name="T"/>.</returns>
        /// <exception cref="HttpRequestException">Thrown if the HTTP response has a non-success status code.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the deserialized content results in a null value.</exception>
        public static async Task<T> ReadContextAs<T>(this HttpResponseMessage response)
        {
            if(!response.IsSuccessStatusCode) throw new HttpRequestException($"Something went wrong calling the API: {response}!");

            string dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);  // Converts JSON to object.

            return JsonSerializer.Deserialize<T>(
                dataAsString,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? throw new InvalidOperationException();
        }

        /// <summary>
        /// Sends a POST request to the specified URL with the provided data serialized as JSON.
        /// </summary>
        /// <typeparam name="T">The type of the data to be serialized and included in the request body.</typeparam>
        /// <param name="client">The <see cref="HttpClient"/> instance used to send the request.</param>
        /// <param name="url">The URL to which the POST request will be sent.</param>
        /// <param name="data">The data to be serialized into JSON and included in the request body.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the HTTP response message.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="client"/> or <paramref name="url"/> is null.</exception>
        public static Task<HttpResponseMessage> PostAsJson<T>(this HttpClient client, string url, T data)
        {
            string dataAsString = JsonSerializer.Serialize(data);
            StringContent content = new(dataAsString);

            content.Headers.ContentType = ContentType;

            return client.PostAsync(url, content);
        }

        /// <summary>
        /// Sends a PUT request to the specified URL with the provided data serialized as JSON content.
        /// </summary>
        /// <typeparam name="T">The type of the data to be serialized and sent in the request body.</typeparam>
        /// <param name="client">The <see cref="HttpClient"/> instance used to send the request.</param>
        /// <param name="url">The URL to which the PUT request is sent.</param>
        /// <param name="data">The data to be serialized and included in the request body.</param>
        /// <returns>A task representing the asynchronous operation, with a result of the HTTP response message.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="client"/>, <paramref name="url"/>, or <paramref name="data"/> is null.</exception>
        public static Task<HttpResponseMessage> PutAsJson<T>(this HttpClient client, string url, T data)
        {
            string dataAsString = JsonSerializer.Serialize(data);
            StringContent content = new(dataAsString);

            content.Headers.ContentType = ContentType;

            return client.PutAsync(url, content);
        }
    }
}