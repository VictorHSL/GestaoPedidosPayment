using GestaoPedidosPayment.Core.Entities;
using System.Linq.Expressions;

namespace GestaoPedidosPayment.Core.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        Task<T> GetByIdAsync(Guid id);
        Task<List<T>> GetAllAsync();
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
    }
}
