using FlashSale.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web.Http.ExceptionHandling;

namespace FlashSale.Infrastructure.Exceptions
{
    public class CustomExceptionHandler 
    {
        private readonly ILogger<CustomExceptionHandler> _logger;
        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(ExceptionHandlerContext context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogInformation(exception, "Exception caught at global level");

            (string Details, string Title, int StatusCode) details = exception switch
            {
                OutOfStockException =>
                (
                    exception.Message,
                    "Out Of Stock",
                    StatusCodes.Status409Conflict
                ),
                NotFoundException =>
                (
                    exception.Message,
                    "Not Found",
                    StatusCodes.Status404NotFound
                ),
                BadRequestException =>
                (
                    exception.Message,
                    "Bad Request",
                    StatusCodes.Status400BadRequest
                ),
                _ =>
                (
                    exception.Message,
                    "Internal Server Error",
                    StatusCodes.Status500InternalServerError
                )
            };

            var problemDetails = new ProblemDetails
            {
                Title = details.Title,
                Detail = details.Details,
                Status = details.StatusCode,
                Instance = context.Request.Path
            };
            context.Response.StatusCode = details.StatusCode;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(json, cancellationToken);

            return true;
        }
    }

}
