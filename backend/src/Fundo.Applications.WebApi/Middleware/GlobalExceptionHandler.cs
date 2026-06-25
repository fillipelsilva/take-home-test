using Fundo.Application.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Fundo.Applications.WebApi.Middleware
{
    internal sealed class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred");

            int statusCode = (int)HttpStatusCode.InternalServerError;
            string title = "Server failure";
            string detail = "An unexpected error occurred.";

            if (exception is LoanManagementException loanManagementException)
            {
                title = loanManagementException.RequestName;
                detail = loanManagementException.Error?.Description ?? "An application error occurred.";
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = statusCode;

            var problemDetails = new
            {
                type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                title,
                status = statusCode,
                detail
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails));
        }
    }
}
