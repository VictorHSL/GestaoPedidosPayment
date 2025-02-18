using AutoMapper;
using GestaoPedidosPayment.AppServices.Shared.Dtos;
using GestaoPedidosPayment.AppServices.Shared.MappingProfiles;
using GestaoPedidosPayment.Core.Entities;
using Xunit;

namespace Tests.AppServices.Shared
{
    public class PaymentMappingProfileTests
    {
        private readonly IMapper _mapper;

        public PaymentMappingProfileTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<PaymentMappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        [Fact(DisplayName = "Payment should map correctly to PaymentDto")]
        public void Payment_ShouldMapTo_PaymentDto()
        {
            // Arrange
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = "ORDER123",
                Amount = 99.99m
            };

            // Act
            var paymentDto = _mapper.Map<PaymentDto>(payment);

            // Assert
            Assert.NotNull(paymentDto);
            Assert.Equal(payment.Id, paymentDto.Id);
            Assert.Equal(payment.OrderId, paymentDto.OrderId);
            Assert.Equal(payment.Amount, paymentDto.Amount);
        }

        [Fact(DisplayName = "CreatePaymentDto should map correctly to Payment")]
        public void CreatePaymentDto_ShouldMapTo_Payment()
        {
            // Arrange
            var createPaymentDto = new CreatePaymentDto
            {
                OrderId = "ORDER456",
                Amount = 150.00m
            };

            // Act
            var payment = _mapper.Map<Payment>(createPaymentDto);

            // Assert
            Assert.NotNull(payment);
            Assert.Equal(createPaymentDto.OrderId, payment.OrderId);
            Assert.Equal(createPaymentDto.Amount, payment.Amount);
            Assert.NotEqual(Guid.Empty, payment.Id);
        }
    }
}
