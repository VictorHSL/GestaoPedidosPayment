using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Shared.Infra;

namespace GestaoPedidosPayment.Core.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>, ITransient
    {
        Task<Payment> GetByOrderIdAsync(string orderId);
    }
}
