using Microsoft.Extensions.Primitives;

namespace GeekShopping.OrderAPI.Shared.Middlewares
{
    /// <summary>
    /// Middleware that ensures each HTTP request has a correlation ID for end-to-end request tracking across distributed systems.
    /// </summary>
    /// <remarks>
    /// The correlation ID is read from the 'X-Correlation-Id' request header if present and valid; otherwise, a new GUID is generated.
    /// The correlation ID is set on both the request and response headers and is also assigned to the HTTP context's trace identifier.
    /// This enables consistent tracking of requests through logs and diagnostics.
    /// Add this middleware early in the pipeline to ensure the correlation ID is available to the following components.
    /// </remarks>
    public class CorrelationIdMiddleware
    {
        private const string HeaderName = "X-Correlation-Id";
        private readonly RequestDelegate _requestDelegate;

        // Constructor:
        /// <summary>
        /// Middleware responsible for managing a correlation ID for HTTP requests to enable consistent request tracking.
        /// </summary>
        /// <remarks>
        /// The correlation ID is retrieved from the incoming request header if present and valid. Otherwise, a new correlation ID is generated.
        /// The ID is then set in the request context, trace identifier, and response headers to facilitate end-to-end tracking in distributed systems.
        /// This middleware ensures each request is uniquely identifiable for logging and diagnostics purposes.
        /// </remarks>
        public CorrelationIdMiddleware(RequestDelegate requestDelegate) => _requestDelegate = requestDelegate;

        // Methods:
        /// <summary>
        /// Processes an HTTP request to ensure that a correlation ID is present.
        /// If a correlation ID is provided in the request headers, it validates and propagates it.
        /// Otherwise, generates a new correlation ID, sets it in the request and response headers, and links it to the trace identifier of the context.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            Guid correlationGuid;

            if(
                (context.Request.Headers.TryGetValue(HeaderName, out StringValues headerValue)) &&
                (Guid.TryParse(headerValue, out Guid parsed))
            )
            {
                correlationGuid = parsed;
            }
            else
            {
                correlationGuid = Guid.Empty;
                context.Request.Headers[HeaderName] = correlationGuid.ToString("D");
            }

            string correlationId = correlationGuid.ToString("D");

            context.TraceIdentifier = correlationId;
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[HeaderName] = correlationId;
                return Task.CompletedTask;
            });

            await _requestDelegate(context);
        }
    }
}