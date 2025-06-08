using System.Net;
using System.Text.Json;
using EnrollmentService.Utils.Exceptions;

namespace EnrollmentService.Middleware
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
            var errors = new List<string>();
            var statusCode = HttpStatusCode.InternalServerError;

            switch (exception)
            {
                case EnrollmentCapacityExceededException capacityEx:
                    errors.Add($"Enrollment capacity exceeded for class. Current: {capacityEx.CurrentCount}, Capacity: {capacityEx.Capacity}");
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case EnrollmentTransactionException transEx:
                    errors.Add("Failed to create enrollments due to capacity constraints");
                    errors.AddRange(transEx.FailedEnrollments.Select(id => $"Failed enrollment for class: {id}"));
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case ArgumentException argEx:
                    errors.Add(argEx.Message);
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case InvalidOperationException invOpEx:
                    errors.Add(invOpEx.Message);
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                default:
                    errors.Add(exception.Message);
                    if (exception.InnerException != null)
                    {
                        errors.Add(exception.InnerException.Message);
                    }
                    statusCode = HttpStatusCode.InternalServerError;
                    break;
            }

            var response = new ApiResponse<List<string>>(false, null, errors);
            var result = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(result);
        }
    }
}
