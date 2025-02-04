using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Repositories;

namespace GestaoPedidosPayment.Core.Shared.Infra
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : BaseEntity;
        Task<bool> CommitAsync();
        void Rollback();
    }
}
