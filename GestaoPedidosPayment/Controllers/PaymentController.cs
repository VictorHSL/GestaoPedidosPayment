using GestaoPedidosPayment.AppServices.Shared.Dtos;
using GestaoPedidosPayment.AppServices.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GestaoPedidosPayment.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentAppService _paymentAppService;

        public PaymentController(IPaymentAppService paymentAppService)
        {
            _paymentAppService = paymentAppService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
        {
            var payment = await _paymentAppService.CreatePayment(createPaymentDto);
            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, payment);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPayment(Guid id)
        {
            var payment = await _paymentAppService.GetPayment(id);
            if (payment == null)
                return NotFound();
            return Ok(payment);
        }

        [HttpGet("byOrderId/{orderId}")]
        public async Task<IActionResult> GetPaymentByOrderId(string orderId)
        {
            var payment = await _paymentAppService.GetPayment(orderId);
            if (payment == null)
                return NotFound();
            return Ok(payment);
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelPayment(Guid id, [FromBody] string reason)
        {
            await _paymentAppService.CancelPayment(id, reason);
            return NoContent();
        }

        [HttpPost("{id}/complete")]
        public async Task<IActionResult> MarkPaymentAsCompleted(Guid id)
        {
            await _paymentAppService.SetCompleted(id);
            return Ok();
        }
    }
}
