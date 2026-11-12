using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.Exceptions;

namespace TestProduct.Api.Middleware
{
    public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            if(exception is OperationCanceledException && context.RequestAborted.IsCancellationRequested)
            {
                return false;
            }

            var (statusCode, title, detail) = exception switch
            {
                NotFoundException ex =>(
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                ex.Message
                ),

                BadRequestException ex => 
                (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                ex.Message
                ),

                ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                "Invalid Request",
                ex.Message
                ),

                ConflictException ex => (
                StatusCodes.Status409Conflict,
                "Conflict",
                ex.Message
                ),
                
                _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred. Please try again later."
                )

            };

            // Log unexpected errors as errors;
            if(statusCode>=500)
            {
                logger.LogError( 
                    exception, 
                    "Unhandled exception. TraceId: {TraceId}", 
                    context.TraceIdentifier);
            }
            else
            {
                logger.LogWarning(
                    "Request failed with status {StatusCode}." +
                    "Message: {Message}. TraceId: {TraceId}",
                    statusCode,
                    exception.Message,
                    context.TraceIdentifier
                    );
            }


            // Don't attempt to replace a response that has already started.
            if (context.Response.HasStarted)
            {
                logger.LogWarning(
                    "Cannot write exception response because the response has started.");
                return false;
            }

            context.Response.Clear();
            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            problemDetails.Extensions["traceId"] =
                context.TraceIdentifier;

            problemDetails.Extensions["timestamp"] =
                DateTime.UtcNow;

            await context.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);
            return true;
        }
    }
}
