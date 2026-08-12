namespace GeekShopping.OrderAPI.Shared.Middlewares
{
    /// <summary>
    /// Middleware that logs details of incoming HTTP requests and their corresponding responses, including the HTTP method, request path, response status
    /// code, and processing duration.
    /// </summary>
    /// <remarks>
    /// Use this middleware to capture request and response information for diagnostic or monitoring purposes in an ASP.NET Core application.
    /// The middleware should be registered in the application's request pipeline, typically early in the pipeline to ensure all requests are logged.
    /// Logging is performed asynchronously and includes the time taken to process each request.
    /// </remarks>
    public sealed class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        // Constructor:
        /// <summary>
        /// Middleware that logs details of incoming HTTP requests and their corresponding responses, including the HTTP method, request path, response
        /// status code, and processing duration.
        /// </summary>
        /// <remarks>
        /// Use this middleware to capture request and response information for diagnostic or monitoring purposes in an ASP.NET Core application.
        /// The middleware should be registered in the application's request pipeline, typically early in the pipeline to ensure all requests are logged.
        /// Logging is performed asynchronously and includes the time taken to process each request.
        /// </remarks>
        public RequestLoggingMiddleware(RequestDelegate requestDelegate, ILogger<RequestLoggingMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        // Method:
        /// <summary>
        /// Logs details of an HTTP request and its response, including the method, path, status code, and the time taken to process the request.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> object representing the HTTP request and response.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            DateTime start = DateTime.UtcNow;

            await _requestDelegate(context);

            TimeSpan duration = (DateTime.UtcNow - start);

            _logger.LogInformation(
                "{method} {path} responded {statusCode} in {duration}ms",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                duration.TotalMilliseconds
            );
        }
    }
}