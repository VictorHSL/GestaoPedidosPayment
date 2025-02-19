using FluentAssertions;
using GestaoPedidosPayment.AppServices;
using GestaoPedidosPayment.AppServices.Shared;
using GestaoPedidosPayment.AppServices.Shared.Dtos;
using GestaoPedidosPayment.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Tests.Controllers
{
    public class PaymentControllerTests
    {
        private readonly IPaymentAppService _paymentAppService;
        private readonly PaymentController _controller;

        public PaymentControllerTests()
        {
            _paymentAppService = Substitute.For<IPaymentAppService>();
            _controller = new PaymentController(_paymentAppService);
        }

        [Fact(DisplayName = "Should return CreatedAtAction when payment is created")]
        public async Task CreatePayment_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var createPaymentDto = new CreatePaymentDto { OrderId = "order-123", Amount = 100m };
            var paymentDto = new PaymentDto { Id = Guid.NewGuid(), OrderId = "order-123", Amount = 100m };

            _paymentAppService.CreatePayment(createPaymentDto).Returns(Task.FromResult(paymentDto));

            // Act
            var result = await _controller.CreatePayment(createPaymentDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(PaymentController.GetPayment), createdAtActionResult.ActionName);
            Assert.Equal(paymentDto.Id, ((PaymentDto)createdAtActionResult.Value).Id);
        }

        [Fact(DisplayName = "Should return Ok when payment exists")]
        public async Task GetPayment_ShouldReturnOk_WhenPaymentExists()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var paymentDto = new PaymentDto { Id = paymentId, OrderId = "order-456", Amount = 50m };

            _paymentAppService.GetPayment(paymentId).Returns(Task.FromResult(paymentDto));

            // Act
            var result = await _controller.GetPayment(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(paymentDto, okResult.Value);
        }

        [Fact(DisplayName = "Should return NotFound when payment does not exist")]
        public async Task GetPayment_ShouldReturnNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            _paymentAppService.GetPayment(Arg.Any<Guid>()).Returns(Task.FromResult<PaymentDto>(null));

            // Act
            var result = await _controller.GetPayment(Guid.NewGuid());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(DisplayName = "Should return Ok when payment exists by OrderId")]
        public async Task GetPaymentByOrderId_ShouldReturnOk_WhenPaymentExists()
        {
            // Arrange
            var orderId = "order-789";
            var paymentDto = new PaymentDto { Id = Guid.NewGuid(), OrderId = orderId, Amount = 75m };

            _paymentAppService.GetPayment(orderId).Returns(Task.FromResult(paymentDto));

            // Act
            var result = await _controller.GetPaymentByOrderId(orderId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(paymentDto, okResult.Value);
        }

        [Fact(DisplayName = "Should return NotFound when payment does not exist by OrderId")]
        public async Task GetPaymentByOrderId_ShouldReturnNotFound_WhenPaymentDoesNotExist()
        {
            // Arrange
            _paymentAppService.GetPayment(Arg.Any<string>()).Returns(Task.FromResult<PaymentDto>(null));

            // Act
            var result = await _controller.GetPaymentByOrderId("invalid-order");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(DisplayName = "Should return NoContent when payment is canceled")]
        public async Task CancelPayment_ShouldReturnNoContent()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var reason = "Customer requested cancellation";

            // Act
            var result = await _controller.CancelPayment(paymentId, reason);

            // Assert
            Assert.IsType<NoContentResult>(result);
            await _paymentAppService.Received(1).CancelPayment(paymentId, reason);
        }

        [Fact(DisplayName = "Should return Ok when payment is marked as completed")]
        public async Task MarkPaymentAsCompleted_ShouldReturnOk()
        {
            // Arrange
            var paymentId = Guid.NewGuid();

            // Act
            var result = await _controller.MarkPaymentAsCompleted(paymentId);

            // Assert
            Assert.IsType<OkResult>(result);
            await _paymentAppService.Received(1).SetCompleted(paymentId);
        }

        [Fact(DisplayName = "GIVEN a valid CreatePaymentDto WHEN CreatePayment is called THEN it should return CreatedAtAction with payment details")]
        public async Task Given_Valid_CreatePaymentDto_When_CreatePayment_Called_Then_Should_Return_CreatedAtAction()
        {
            // GIVEN a valid CreatePaymentDto
            var createPaymentDto = new CreatePaymentDto
            {
                OrderId = "ORDER123",
                Amount = 99.99m
            };

            var paymentDto = new PaymentDto
            {
                Id = Guid.NewGuid(),
                OrderId = createPaymentDto.OrderId,
                Amount = createPaymentDto.Amount
            };

            _paymentAppService.CreatePayment(createPaymentDto).ReturnsForAnyArgs(paymentDto);

            // WHEN CreatePayment is called
            var result = await _controller.CreatePayment(createPaymentDto);

            // THEN it should return CreatedAtAction with payment details
            result.Should().BeOfType<CreatedAtActionResult>();

            var createdAtResult = result as CreatedAtActionResult;
            createdAtResult!.ActionName.Should().Be(nameof(PaymentController.GetPayment));
            createdAtResult.RouteValues!["id"].Should().Be(paymentDto.Id);
            createdAtResult.Value.Should().BeEquivalentTo(paymentDto);
        }
    }
}
