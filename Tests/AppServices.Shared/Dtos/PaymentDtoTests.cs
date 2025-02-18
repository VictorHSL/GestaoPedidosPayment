using GestaoPedidosPayment.AppServices.Shared.Dtos;
using GestaoPedidosPayment.Core.Shared.ValueObjects;
using Xunit;

namespace Tests.AppServices.Shared.Dtos
{
    public class PaymentDtoTests
    {
        [Fact(DisplayName = "PaymentDto should initialize with default values")]
        public void PaymentDto_ShouldInitialize_WithDefaultValues()
        {
            // Arrange
            var paymentDto = new PaymentDto();

            // Assert base class properties
            Assert.Equal(Guid.Empty, paymentDto.Id); 
            Assert.Equal(default(DateTime), paymentDto.CreatedAt); 

            // Assert PaymentDto properties
            Assert.Null(paymentDto.PaymentId); 
            Assert.Null(paymentDto.OrderId); 
            Assert.Equal(0m, paymentDto.Amount); 
            Assert.Equal(PaymentStatus.Pending, paymentDto.Status); 
        }

        [Fact(DisplayName = "PaymentDto should set properties correctly when assigned")]
        public void PaymentDto_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var expectedId = Guid.NewGuid();
            var expectedCreatedAt = DateTime.UtcNow;
            var expectedPaymentId = "payment123";
            var expectedOrderId = "order123";
            var expectedAmount = 99.99m;
            var expectedStatus = PaymentStatus.Completed;

            var paymentDto = new PaymentDto
            {
                Id = expectedId,
                CreatedAt = expectedCreatedAt,
                PaymentId = expectedPaymentId,
                OrderId = expectedOrderId,
                Amount = expectedAmount,
                Status = expectedStatus
            };

            // Assert base class properties
            Assert.Equal(expectedId, paymentDto.Id);
            Assert.Equal(expectedCreatedAt, paymentDto.CreatedAt);

            // Assert PaymentDto properties
            Assert.Equal(expectedPaymentId, paymentDto.PaymentId);
            Assert.Equal(expectedOrderId, paymentDto.OrderId);
            Assert.Equal(expectedAmount, paymentDto.Amount);
            Assert.Equal(expectedStatus, paymentDto.Status);
        }

        [Fact(DisplayName = "PaymentDto should assign default values when instantiated")]
        public void PaymentDto_ShouldAssignDefaultValues_WhenInstantiated()
        {
            // Arrange & Act
            var paymentDto = new PaymentDto();

            // Assert base class properties
            Assert.Equal(Guid.Empty, paymentDto.Id);
            Assert.Equal(default(DateTime), paymentDto.CreatedAt);

            // Assert PaymentDto properties
            Assert.Null(paymentDto.PaymentId);
            Assert.Null(paymentDto.OrderId);
            Assert.Equal(0m, paymentDto.Amount);
            Assert.Equal(PaymentStatus.Pending, paymentDto.Status);
        }
    }
}
