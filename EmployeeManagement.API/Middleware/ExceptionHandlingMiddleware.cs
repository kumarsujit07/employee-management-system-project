using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using EmployeeManagement.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.API.Middleware
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

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during request execution.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred on the server.";

            // Catch specific domain errors to return correct status codes
            switch (exception)
            {
                case KeyNotFoundException:
                case EmployeeService.EmployeeNotFoundException:
                case AttendanceService.AttendanceNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    message = exception.Message;
                    break;
                case AuthService.InvalidCredentialsException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = exception.Message;
                    break;
                case EmployeeService.DuplicateEmailException:
                case AttendanceService.DuplicateCheckInException:
                case ArgumentException:
                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
            }

            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                success = false,
                message = message
            });

            return context.Response.WriteAsync(result);
        }
    }
}
