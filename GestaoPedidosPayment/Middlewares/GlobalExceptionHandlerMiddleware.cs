using GestaoPedidosPayment.Core.Shared.Infra;
using Newtonsoft.Json;

namespace GestaoPedidosPayment.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUnitOfWork unitOfWork)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    ErrorMessage = "An unexpected error occurred. Please try again later.",
                    ExceptionMessage = ex.ToString()
                }));
            }
        }
    }
}
