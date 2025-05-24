using System.Net;
using System.Text.Json;

namespace UserService.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var errors = new List<string> { exception.Message };

            if (exception.InnerException != null)
            {
                errors.Add(exception.InnerException.Message);
            }

            // Set appropriate status code based on exception type
            context.Response.StatusCode = exception switch
            {
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new ApiResponse<List<string>>(false, null, errors);
            var result = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(result);
        }
    }
}
