using Serilog;
using System.Diagnostics;
using System.Net;

namespace WarshipImport.Middleware
{
	internal class GlobalExceptionHandler
	{
		private readonly RequestDelegate _next;

		public GlobalExceptionHandler(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleException(context, ex);
			}
		}

		private Task HandleException(HttpContext context, Exception ex)
		{
			var errorId = Guid.NewGuid();

			Log.Fatal(ex, $"Fatal Exception: {errorId}");

			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			Debugger.Break();

			return context.Response.WriteAsJsonAsync(new
			{
				ErrorId = errorId,
				Message = "Internal Exception, please contact support"
			});
		}
	}
}