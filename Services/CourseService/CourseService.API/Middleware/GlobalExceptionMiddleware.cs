using CourseService.Domain.Exceptions;
using System.Diagnostics;
using System.Net;

namespace CourseService.API.Middleware
{
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;
		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception ex)
		{
			var (statusCode, message) = ex switch
			{
				CourseNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
				SectionNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
				LessonNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
				UnauthorizedCourseAccessException => (StatusCodes.Status403Forbidden, ex.Message),
				UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ex.Message),
				InvalidOperationException => (StatusCodes.Status400BadRequest, ex.Message),
				ArgumentException => (StatusCodes.Status400BadRequest, ex.Message),
				_ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
			};

			if(statusCode == (int)HttpStatusCode.InternalServerError)
				_logger.LogError(ex, "unhandled exceptions");
			else
				_logger.LogWarning(ex, $"handled exceptions: {message}", ex.Message);

			context.Response.StatusCode = (int)statusCode;
			context.Response.ContentType = "application/json";

			var response = new
			{
				statusCode = statusCode,
				message = message,
				traceId = Activity.Current?.Id ?? context.TraceIdentifier
			};

			await context.Response.WriteAsJsonAsync(response);
		}
	}
}
