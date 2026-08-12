using System.Net;
using System.Text.Json;

namespace GeekShopping.OrderAPI.Shared.Middlewares
{
    /// <summary>
    /// Middleware that intercepts unhandled exceptions during HTTP request processing, logs the exception details, and returns a standardized JSON error response to the client.
    /// </summary>
    /// <remarks>
    /// Use this middleware to provide consistent error handling and logging for unhandled exceptions in the ASP.NET Core request pipeline.
    /// Place it early in the middleware pipeline to ensure that exceptions from subsequent components are properly caught and handled.
    /// The middleware sets the response status code to 500 (Internal Server Error) and includes an error message and trace identifier in the JSON response body.
    /// </remarks>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        // Constructor:
        /// <summary>
        /// Middleware that intercepts unhandled exceptions during HTTP request processing, logs the exception details, and returns a standardized JSON error response to the client.
        /// </summary>
        /// <remarks>
        /// Use this middleware to provide consistent error handling and logging for unhandled exceptions in the ASP.NET Core request pipeline.
        /// Place it early in the middleware pipeline to ensure that exceptions from later components are properly caught and handled.
        /// The middleware sets the response status code to 500 (Internal Server Error) and includes an error message and trace identifier in the JSON response body.
        /// </remarks>
        public ExceptionHandlingMiddleware(RequestDelegate requestDelegate, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }

        // Method:
        /// <summary>
        /// Processes an HTTP request, intercepts unhandled exceptions, logs them, and returns a standardized JSON error response to the client if an exception occurs during request processing.
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/> representing the current HTTP request and response context.</param>
        /// <returns>A task representing the asynchronous operation of processing the HTTP request.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch(Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception caught by middleware!");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                string exceptionMessage = (exception.InnerException != null)
                    ? exception.InnerException.Message
                    : exception.Message;

                var response = new
                {
                    error = "Internal Server Error",
                    message = exceptionMessage,
                    traceId = context.TraceIdentifier
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}