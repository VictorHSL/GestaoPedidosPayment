using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Shared.Infra;

namespace GestaoPedidosPayment.Core.Managers.Interfaces
{
    public interface IPaymentManager : ITransient
    {
        Task<Payment> CreatePayment(string orderId, decimal amount);
        Task<Payment> GetPayment(Guid id);
        Task<Payment> GetPayment(string orderId);
        Task CancelPayment(Guid id, string? reason = null);
        Task CancelPayment(string orderId, string? reason = null);
        Task SetCompleted(Guid paymentId);
        Task SetCompleted(string orderId);
        Task SetFailed(Guid paymentId, string? failureReason = null);
        Task SetFailed(string orderId, string? failureReason = null);
    }
}
