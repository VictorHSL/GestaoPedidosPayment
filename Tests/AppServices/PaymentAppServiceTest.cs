using AutoMapper;
using GestaoPedidosPayment.AppServices.Shared.Dtos;
using GestaoPedidosPayment.AppServices;
using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Managers.Interfaces;
using GestaoPedidosPayment.Core.Shared.ValueObjects;
using NSubstitute;
using Xunit;

namespace Tests.AppServices
{
    public class PaymentAppServiceTest
    {
        private readonly IPaymentManager _paymentManager;
        private readonly IMapper _mapper;
        private readonly PaymentAppService _paymentAppService;

        public PaymentAppServiceTest()
        {
            _paymentManager = Substitute.For<IPaymentManager>();
            _mapper = Substitute.For<IMapper>();
            _paymentAppService = new PaymentAppService(_paymentManager, _mapper);
        }

        [Fact(DisplayName = "Should create a payment and return PaymentDto")]
        public async Task CreatePayment_ShouldReturnPaymentDto_WhenSuccessful()
        {
            var createDto = new CreatePaymentDto { OrderId = "123", Amount = 100 };
            var payment = new Payment { OrderId = "123", Amount = 100, Status = PaymentStatus.Pending };
            var expectedDto = new PaymentDto { OrderId = "123", Amount = 100, Status = PaymentStatus.Pending };

            _paymentManager.CreatePayment(createDto.OrderId, createDto.Amount).Returns(payment);
            _mapper.Map<PaymentDto>(payment).Returns(expectedDto);

            var result = await _paymentAppService.CreatePayment(createDto);

            Assert.Equal(expectedDto, result);
        }

        [Fact(DisplayName = "Should retrieve a payment by Id and return PaymentDto")]
        public async Task GetPayment_ById_ShouldReturnPaymentDto_WhenFound()
        {
            var paymentId = Guid.NewGuid();
            var payment = new Payment { Id = paymentId, OrderId = "123", Amount = 100 };
            var expectedDto = new PaymentDto { OrderId = "123", Amount = 100 };

            _paymentManager.GetPayment(paymentId).Returns(payment);
            _mapper.Map<PaymentDto>(payment).Returns(expectedDto);

            var result = await _paymentAppService.GetPayment(paymentId);

            Assert.Equal(expectedDto, result);
        }

        [Fact(DisplayName = "Should retrieve a payment by OrderId and return PaymentDto")]
        public async Task GetPayment_ByOrderId_ShouldReturnPaymentDto_WhenFound()
        {
            var orderId = "123";
            var payment = new Payment { OrderId = orderId, Amount = 100 };
            var expectedDto = new PaymentDto { OrderId = orderId, Amount = 100 };

            _paymentManager.GetPayment(orderId).Returns(payment);
            _mapper.Map<PaymentDto>(payment).Returns(expectedDto);

            var result = await _paymentAppService.GetPayment(orderId);

            Assert.Equal(expectedDto, result);
        }

        [Fact(DisplayName = "Should cancel a payment by Id successfully")]
        public async Task CancelPayment_ById_ShouldCallManager()
        {
            var paymentId = Guid.NewGuid();
            await _paymentAppService.CancelPayment(paymentId, "Test Reason");

            await _paymentManager.Received(1).CancelPayment(paymentId, "Test Reason");
        }

        [Fact(DisplayName = "Should cancel a payment by OrderId successfully")]
        public async Task CancelPayment_ByOrderId_ShouldCallManager()
        {
            var orderId = "123";
            await _paymentAppService.CancelPayment(orderId, "Test Reason");

            await _paymentManager.Received(1).CancelPayment(orderId, "Test Reason");
        }

        [Fact(DisplayName = "SetCompleted should call SetCompleted on IPaymentManager")]
        public async Task SetCompleted_ShouldCallSetCompleted_OnIPaymentManager()
        {
            // Arrange
            var paymentId = Guid.NewGuid();

            // Act
            await _paymentAppService.SetCompleted(paymentId);

            // Assert
            await _paymentManager.Received(1).SetCompleted(paymentId);
        }
    }
}
