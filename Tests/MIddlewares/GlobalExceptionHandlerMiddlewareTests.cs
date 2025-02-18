using GestaoPedidosPayment.Core.Shared.Infra;
using GestaoPedidosPayment.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Tests.MIddlewares
{
    public class GlobalExceptionHandlerMiddlewareTests
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly GlobalExceptionHandlerMiddleware _middleware;

        public GlobalExceptionHandlerMiddlewareTests()
        {
            _next = Substitute.For<RequestDelegate>();
            _logger = Substitute.For<ILogger<GlobalExceptionHandlerMiddleware>>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _middleware = new GlobalExceptionHandlerMiddleware(_next, _logger);
        }

        [Fact(DisplayName = "Should call next middleware when no exception occurs")]
        public async Task InvokeAsync_ShouldCallNextMiddleware_WhenNoExceptionOccurs()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            await _middleware.InvokeAsync(context, _unitOfWork);

            // Assert
            await _next.Received(1).Invoke(context);
        }

        [Fact(DisplayName = "Should handle exception and return 500 status code")]
        public async Task InvokeAsync_ShouldHandleException_AndReturn500StatusCode()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var exception = new Exception("Test exception");

            _next.When(x => x.Invoke(Arg.Any<HttpContext>())).Throw(exception);
            context.Response.Body = new MemoryStream(); // Set response body to test output

            // Act
            await _middleware.InvokeAsync(context, _unitOfWork);

            // Assert
            Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);

            // Read response body
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
            var errorResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);

            Assert.NotNull(errorResponse);
            Assert.Equal("An unexpected error occurred. Please try again later.", (string)errorResponse.ErrorMessage);
            Assert.Contains("Test exception", (string)errorResponse.ExceptionMessage);

            // Ensure logger logs the exception
            _logger.Received(1).LogError(exception, "An unhandled exception occurred.");
        }
    }
}
