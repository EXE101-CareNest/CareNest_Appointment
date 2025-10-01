using CareNest_Appointment.Application.Exceptions;
using CareNest_Appointment.Domain.Commons.Base;
using System.Net;
using System.Text.Json;

namespace CareNest_Appointment.API.Middleware
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // gọi middleware tiếp theo
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception caught by middleware.");
                await HandleExceptionAsync(context, ex);
            }
        }

        //BadRequest chuỗi, xuống dòng cho dễ xem
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            object errorDetails;

            if (exception is BadRequestException badRequest)
            {
                statusCode = (int)HttpStatusCode.BadRequest;

                errorDetails = new
                {
                    title = "Validation failed",
                    errors = badRequest.Errors.Any()
                    ? badRequest.Errors
                    : new List<string> { badRequest.Message }
                };
            }
            else if (exception is BaseException.ErrorException errorException)
            {
                statusCode = errorException.StatusCode;
                errorDetails = new
                {
                    title = errorException.ErrorDetail.ErrorMessage?.ToString(),
                    details = errorException.ErrorDetail.ErrorCode
                };
            }
            else if (exception is InternalException)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorDetails = new
                {
                    title = exception.Message,
                    details = exception.InnerException?.Message
                };
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                errorDetails = new
                {
                    title = "An unexpected error occurred.",
                    details = exception.Message
                };
            }

            var response = new
            {
                error = errorDetails,
                statusCode,
                timestamp = DateTime.UtcNow
            };

            string payload = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            return context.Response.WriteAsync(payload);
        }
    }
}
