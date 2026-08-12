namespace GeekShopping.OrderAPI.Shared.Middlewares
{
    /// <summary>
    /// Middleware that measures the execution time of HTTP requests and logs a warning if a request exceeds a predefined duration threshold.
    /// </summary>
    /// <remarks>
    /// This middleware is intended to be added to the ASP.NET Core request pipeline to help identify slow requests.
    /// If a request takes longer than 500 milliseconds to complete, a warning is logged using the provided logger.
    /// This can be useful for monitoring application performance and diagnosing potential bottlenecks.
    /// </remarks>
    public sealed class PerformanceMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<PerformanceMiddleware> _logger;
        private const int ThresholdMs = 500;

        // Constructor:
        /// <summary>
        /// Middleware that measures the execution time of HTTP requests and logs a warning if a request exceeds a predefined duration threshold.
        /// </summary>
        /// <remarks>
        /// This middleware helps monitor and diagnose slow HTTP requests in the application.
        /// It logs a warning if the processing time surpasses the configured threshold in milliseconds.
        /// </remarks>
        public PerformanceMiddleware(RequestDelegate requestDelegate, ILogger<PerformanceMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        // Method:
        /// <summary>
        /// Handles an HTTP request, measures its execution time, and logs a warning if the request exceeds the specified threshold.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task Invoke(HttpContext context)
        {
            DateTime start = DateTime.UtcNow;

            await _requestDelegate(context);

            TimeSpan duration = DateTime.UtcNow - start;

            if(duration.TotalMilliseconds > ThresholdMs)
            {
                _logger.LogWarning(
                    "Slow request detected: {method} {path} took {duration}ms",
                    context.Request.Method,
                    context.Request.Path,
                    duration.TotalMilliseconds
                );
            }
        }
    }
}
