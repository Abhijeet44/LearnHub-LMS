using AuthService.Domain.Exception;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace AuthService.API.Middleware
{
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;

		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
		{
			this._next = next;
			this._logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception ex)
		{
			var (statusCode, message) = ex switch
			{
				UserNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
				InvalidCredentialException => (StatusCodes.Status401Unauthorized, ex.Message),
				UnauthorizedAccessException => (StatusCodes.Status403Forbidden, ex.Message),
				ApplicationException => (StatusCodes.Status400BadRequest, ex.Message),	
				_ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
			};

			if(statusCode == StatusCodes.Status500InternalServerError)
				_logger.LogError(ex, "An unexpected error occurred.");
			else
				_logger.LogWarning(ex, "A handled exception occurred: {Message}", ex.Message);

			context.Response.StatusCode = (int)statusCode;
			context.Response.ContentType = "application/json";

			var response = new
			{
			    status = statusCode,
				Error = message,
				traceId = context.TraceIdentifier
			};
			 await context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
	}
}
