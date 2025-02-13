using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Managers.Interfaces;
using GestaoPedidosPayment.Core.Repositories;
using GestaoPedidosPayment.Core.Shared.Infra;
using GestaoPedidosPayment.Core.Shared.ValueObjects;

namespace GestaoPedidosPayment.Core.Managers
{
    public class PaymentManager : IPaymentManager
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentManager(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
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

            await _paymentRepository.AddAsync(payment);
            return payment;
        }

        public async Task SetCompleted(Guid paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) throw new Exception("Payment not found");

            payment.Status = PaymentStatus.Completed;
            await _paymentRepository.UpdateAsync(payment);
        }

        public async Task SetCompleted(string orderId)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            payment.Status = PaymentStatus.Completed;
            await _paymentRepository.UpdateAsync(payment);
        }

        public Task<Payment> GetPayment(string orderId)
        {
            return _paymentRepository.GetByOrderIdAsync(orderId);
        }

        public async Task CancelPayment(Guid paymentId, string? reason = null)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            if (payment == null) throw new Exception("Payment not found");

            payment.Status = PaymentStatus.Canceled;
            payment.Comments = reason;
            await _paymentRepository.UpdateAsync(payment);
        }

        public async Task CancelPayment(string orderId, string? reason = null)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            payment.Status = PaymentStatus.Canceled;
            payment.Comments = reason;
            await _paymentRepository.UpdateAsync(payment);
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
    }

}
