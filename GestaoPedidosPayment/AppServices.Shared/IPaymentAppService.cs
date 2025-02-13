using GestaoPedidosPayment.AppServices.Shared.Dtos;
using GestaoPedidosPayment.Core.Shared.Infra;

namespace GestaoPedidosPayment.AppServices.Shared
{
    public interface IPaymentAppService : ITransient
    {
        Task<PaymentDto> CreatePayment(CreatePaymentDto createPaymentDto);
        Task<PaymentDto> GetPayment(Guid id);
        Task<PaymentDto> GetPayment(string orderId);
        Task CancelPayment(Guid id, string? reason = null);
        Task CancelPayment(string orderId, string? reason = null);
    }
}
