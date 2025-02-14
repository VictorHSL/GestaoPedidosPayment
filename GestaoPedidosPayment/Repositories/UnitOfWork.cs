using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Repositories;
using GestaoPedidosPayment.Core.Shared.Infra;
using MongoDB.Driver;

namespace GestaoPedidosPayment.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoDatabase _database;
        private readonly Dictionary<Type, object> _repositories = new();
        private bool _disposed = false;

        public UnitOfWork(IMongoClient client, string databaseName)
        {
            _database = client.GetDatabase(databaseName);
        }

        public IRepository<T> GetRepository<T>() where T : BaseEntity
        {
            if (!_repositories.ContainsKey(typeof(T)))
            {
                _repositories[typeof(T)] = new BaseMongoRepository<T>(_database, typeof(T).Name);
            }

            return (IRepository<T>)_repositories[typeof(T)];
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }

}
