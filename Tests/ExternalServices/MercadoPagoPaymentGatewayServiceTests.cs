using GestaoPedidosPayment.ExternalServices;
using Xunit;

namespace Tests.ExternalServices
{
    public class MercadoPagoPaymentGatewayServiceTests
    {
        private readonly MercadoPagoPaymentGatewayService _paymentGatewayService;

        public MercadoPagoPaymentGatewayServiceTests()
        {
            _paymentGatewayService = new MercadoPagoPaymentGatewayService();
        }

        [Fact(DisplayName = "Should create a payment and return a payment ID")]
        public async Task CreatePaymentAsync_ShouldReturnPaymentId()
        {
            // Arrange
            decimal amount = 100.0m;
            string id = "order-123";

            // Act
            string paymentId = await _paymentGatewayService.CreatePaymentAsync(amount, id);

            // Assert
            Assert.False(string.IsNullOrEmpty(paymentId));
        }

        [Fact(DisplayName = "Should cancel an existing payment")]
        public async Task CancelPaymentAsync_ShouldCancelPayment_WhenPaymentExists()
        {
            // Arrange
            string paymentId = await _paymentGatewayService.CreatePaymentAsync(50.0m, "order-456");

            // Act
            bool result = await _paymentGatewayService.CancelPaymentAsync(paymentId);

            // Assert
            Assert.True(result);

            // Verify the status is updated
            string status = await _paymentGatewayService.GetPaymentStatusAsync(paymentId);
            Assert.Equal("canceled", status);
        }

        [Fact(DisplayName = "Should return false when canceling a non-existent payment")]
        public async Task CancelPaymentAsync_ShouldReturnFalse_WhenPaymentDoesNotExist()
        {
            // Act
            bool result = await _paymentGatewayService.CancelPaymentAsync("invalid-id");

            // Assert
            Assert.False(result);
        }

        [Fact(DisplayName = "Should return the correct payment status")]
        public async Task GetPaymentStatusAsync_ShouldReturnCorrectStatus()
        {
            // Arrange
            string paymentId = await _paymentGatewayService.CreatePaymentAsync(75.0m, "order-789");

            // Act
            string status = await _paymentGatewayService.GetPaymentStatusAsync(paymentId);

            // Assert
            Assert.Equal("pending", status);
        }

        [Fact(DisplayName = "Should return 'not_found' for a non-existent payment")]
        public async Task GetPaymentStatusAsync_ShouldReturnNotFound_WhenPaymentDoesNotExist()
        {
            // Act
            string status = await _paymentGatewayService.GetPaymentStatusAsync("invalid-id");

            // Assert
            Assert.Equal("not_found", status);
        }
    }
}
