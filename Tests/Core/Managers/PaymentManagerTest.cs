using System.Linq.Expressions;
using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Managers;
using GestaoPedidosPayment.Core.Repositories;
using GestaoPedidosPayment.Core.Shared.Infra;
using GestaoPedidosPayment.Core.Shared.ValueObjects;
using GestaoPedidosPayment.ExternalServices;
using Microsoft.Extensions.Configuration;
using Xunit;
using NSubstitute;

namespace Tests.Core.Managers
{
    public class PaymentManagerTest
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly IConfiguration _configuration;
        private readonly PaymentManager _paymentManager;

        public PaymentManagerTest()
        {
            _paymentRepository = Substitute.For<IPaymentRepository>();
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _configuration = Substitute.For<IConfiguration>();
            _paymentGatewayService = Substitute.For<IPaymentGatewayService>();
            _paymentManager = new PaymentManager(_paymentRepository, _unitOfWork, _paymentGatewayService, new HttpClient(), _configuration);
        }

        [Fact(DisplayName = "Should throw InvalidOperationException when OrderId already exists")]
        public async Task CreatePayment_ShouldThrowException_WhenOrderIdExists()
        {
            var orderId = "123";
            _paymentRepository.FindOneAsync(Arg.Any<Expression<Func<Payment, bool>>>()).Returns(new Payment { OrderId = orderId });

            await Assert.ThrowsAsync<InvalidOperationException>(() => _paymentManager.CreatePayment(orderId, 100));
        }

        [Fact(DisplayName = "Should create a new payment when OrderId does not exist")]
        public async Task CreatePayment_ShouldCreatePayment_WhenOrderIdDoesNotExist()
        {
            var orderId = "123";
            _paymentRepository.FindOneAsync(Arg.Any<Expression<Func<Payment, bool>>>()).Returns((Payment)null);

            var payment = await _paymentManager.CreatePayment(orderId, 100);

            Assert.Equal(orderId, payment.OrderId);
            Assert.Equal(100, payment.Amount);
            Assert.Equal(PaymentStatus.Pending, payment.Status);
            await _paymentRepository.Received(1).AddAsync(Arg.Any<Payment>());
        }

        [Fact(DisplayName = "Should throw Exception when trying to complete a non-existent payment")]
        public async Task SetCompleted_ShouldThrowException_WhenPaymentNotFound()
        {
            _paymentRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((Payment)null);

            await Assert.ThrowsAsync<Exception>(() => _paymentManager.SetCompleted(Guid.NewGuid()));
        }

        [Fact(DisplayName = "Should update status to Completed when payment exists")]
        public async Task SetCompleted_ShouldUpdateStatus_WhenPaymentExists()
        {
            var payment = new Payment { Status = PaymentStatus.Pending };
            _paymentRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(payment);

            await _paymentManager.SetCompleted(Guid.NewGuid());

            Assert.Equal(PaymentStatus.Completed, payment.Status);
            await _paymentRepository.Received(1).UpdateAsync(payment);
        }

        [Fact(DisplayName = "Should update status to Completed when payment exists by OrderId")]
        public async Task SetCompleted_ByOrderId_ShouldUpdateStatus_WhenPaymentExists()
        {
            var payment = new Payment { Status = PaymentStatus.Pending };
            _paymentRepository.GetByOrderIdAsync(Arg.Any<string>()).Returns(payment);

            await _paymentManager.SetCompleted("123");

            Assert.Equal(PaymentStatus.Completed, payment.Status);
            await _paymentRepository.Received(1).UpdateAsync(payment);
        }

        [Fact(DisplayName = "Should throw Exception when orderId-based payment is not found")]
        public async Task SetCompleted_ByOrderId_ShouldThrowException_WhenPaymentNotFound()
        {
            _paymentRepository.GetByOrderIdAsync(Arg.Any<string>()).Returns((Payment)null);

            await Assert.ThrowsAsync<NullReferenceException>(() => _paymentManager.SetCompleted("123"));
        }

        [Fact(DisplayName = "Should return a payment when found by OrderId")]
        public async Task GetPayment_ShouldReturnPayment_WhenFoundByOrderId()
        {
            var payment = new Payment { OrderId = "123" };
            _paymentRepository.GetByOrderIdAsync("123").Returns(payment);

            var result = await _paymentManager.GetPayment("123");

            Assert.Equal(payment, result);
        }

        [Fact(DisplayName = "Should throw Exception when trying to cancel a non-existent payment")]
        public async Task CancelPayment_ShouldThrowException_WhenPaymentNotFound()
        {
            _paymentRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((Payment)null);

            await Assert.ThrowsAsync<Exception>(() => _paymentManager.CancelPayment(Guid.NewGuid(), "Reason"));
        }

        [Fact(DisplayName = "Should update status to Canceled with a reason when payment exists")]
        public async Task CancelPayment_ShouldUpdateStatusToCanceled_WhenPaymentExists()
        {
            var payment = new Payment { Status = PaymentStatus.Pending };
            _paymentRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(payment);

            await _paymentManager.CancelPayment(Guid.NewGuid(), "Test Reason");

            Assert.Equal(PaymentStatus.Canceled, payment.Status);
            Assert.Equal("Test Reason", payment.Comments);
            await _paymentRepository.Received(1).UpdateAsync(payment);
        }

        [Fact(DisplayName = "Should update status to Canceled with a reason when payment exists by OrderId")]
        public async Task CancelPayment_ByOrderId_ShouldUpdateStatusToCanceled_WhenPaymentExists()
        {
            var payment = new Payment { Status = PaymentStatus.Pending };
            _paymentRepository.GetByOrderIdAsync(Arg.Any<string>()).Returns(payment);

            await _paymentManager.CancelPayment("123", "Test Reason");

            Assert.Equal(PaymentStatus.Canceled, payment.Status);
            Assert.Equal("Test Reason", payment.Comments);
            await _paymentRepository.Received(1).UpdateAsync(payment);
        }

        [Fact(DisplayName = "Should update status to Failed with a reason when payment exists")]
        public async Task SetFailed_ShouldUpdateStatusToFailed_WhenPaymentExists()
        {
            var payment = new Payment { Status = PaymentStatus.Pending };
            _paymentRepository.GetByIdAsync(Arg.Any<Guid>()).Returns(payment);

            await _paymentManager.SetFailed(Guid.NewGuid(), "Failed Reason");

            Assert.Equal(PaymentStatus.Failed, payment.Status);
            Assert.Equal("Failed Reason", payment.Comments);
            await _paymentRepository.Received(1).UpdateAsync(payment);
        }

        [Fact(DisplayName = "Should update status to Failed with a reason when payment exists by OrderId")]
        public async Task SetFailed_ByOrderId_ShouldUpdateStatusToFailed_WhenPaymentExists()
        {
            var payment = new Payment { Status = PaymentStatus.Pending };
            _paymentRepository.GetByOrderIdAsync(Arg.Any<string>()).Returns(payment);

            await _paymentManager.SetFailed("123", "Failure Reason");

            Assert.Equal(PaymentStatus.Failed, payment.Status);
            Assert.Equal("Failure Reason", payment.Comments);
            await _paymentRepository.Received(1).UpdateAsync(payment);
        }

        [Fact(DisplayName = "Should return a payment when found by PaymentId")]
        public async Task GetPayment_ShouldReturnPayment_WhenFoundByPaymentId()
        {
            var paymentId = Guid.NewGuid();
            var payment = new Payment { Id = paymentId };
            _paymentRepository.GetByIdAsync(paymentId).Returns(payment);

            var result = await _paymentManager.GetPayment(paymentId);

            Assert.Equal(payment, result);
        }

        [Fact(DisplayName = "Should throw Exception when payment is not found by PaymentId")]
        public async Task GetPayment_ShouldThrowException_WhenPaymentNotFoundByPaymentId()
        {
            _paymentRepository.GetByIdAsync(Arg.Any<Guid>()).Returns((Payment)null);

            await Assert.ThrowsAsync<Exception>(() => _paymentManager.GetPayment(Guid.NewGuid()));
        }
    }
}
