using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Managers.Interfaces;
using GestaoPedidosPayment.Core.Repositories;
using GestaoPedidosPayment.Core.Shared.Infra;
using GestaoPedidosPayment.Core.Shared.ValueObjects;
using GestaoPedidosPayment.ExternalServices;
using System.Text.Json;
using System.Text;

namespace GestaoPedidosPayment.Core.Managers
{
    public class PaymentManager : IPaymentManager
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGatewayService _paymentGatewayService;
        private readonly HttpClient _httpClient;
        private readonly string _webHookUrl;

        public PaymentManager(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork,
            IPaymentGatewayService paymentGatewayService, 
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _paymentGatewayService = paymentGatewayService;
            _httpClient = httpClient;
            _webHookUrl = configuration["WebHookUrl"];
        }

        public async Task<Payment> CreatePayment(string orderId, decimal amount)
        {
            var existingPayment = await _paymentRepository.FindOneAsync(p => p.OrderId == orderId);
            if (existingPayment != null)
            {
                throw new InvalidOperationException("A payment with the same OrderId already exists.");
            }

            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Status = PaymentStatus.Pending
            };

            try
            {
                var paymentGatewayId =
                    await _paymentGatewayService.CreatePaymentAsync(payment.Amount, payment.Id.ToString());

                payment.SetPaymentId(paymentGatewayId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"There was an issue with the payment gateway. {ex.Message}");
            }

            await _paymentRepository.AddAsync(payment);

            return payment;
        }

        public async Task SetCompleted(Guid paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) throw new Exception("Payment not found");

            await SetCompleted(payment);
        }

        public async Task SetCompleted(string orderId)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            await SetCompleted(payment);
        }

        public Task<Payment> GetPayment(string orderId)
        {
            return _paymentRepository.GetByOrderIdAsync(orderId);
        }

        public async Task CancelPayment(Guid paymentId, string? reason = null)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) throw new Exception("Payment not found");

            await CancelPayment(payment, reason);
        }

        public async Task CancelPayment(string orderId, string? reason = null)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            await CancelPayment(payment, reason);
        }

        public async Task SetFailed(Guid paymentId, string failureReason)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) throw new Exception("Payment not found");

            payment.Status = PaymentStatus.Failed;
            payment.Comments = failureReason;
            await _paymentRepository.UpdateAsync(payment);
        }
        public async Task SetFailed(string orderId, string? failureReason = null)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            payment.Status = PaymentStatus.Failed;
            payment.Comments = failureReason;
            await _paymentRepository.UpdateAsync(payment);
        }

        public async Task<Payment> GetPayment(Guid paymentId)
        {
            return await _paymentRepository.GetByIdAsync(paymentId)
                   ?? throw new Exception("Payment not found");
        }

        private async Task CancelPayment(Payment payment, string? reason = null)
        {
            try
            {
                await _paymentGatewayService.CancelPaymentAsync(payment.PaymentId);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"There was an issue with the payment gateway. {ex.Message}");
            }

            payment.Status = PaymentStatus.Canceled;
            payment.Comments = reason;
            await _paymentRepository.UpdateAsync(payment);
        }

        private async Task SetCompleted(Payment payment)
        {
            payment.Status = PaymentStatus.Completed;
            await _paymentRepository.UpdateAsync(payment);

            if (!string.IsNullOrEmpty(_webHookUrl))
            {
                var url = $"{_webHookUrl}/{payment.OrderId}";
                var content = new StringContent(string.Empty, Encoding.UTF8, "application/json");

                await _httpClient.PutAsync(url, content);
            }
        }
    }

}
