using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Repositories;
using GestaoPedidosPayment.Core.Shared.Infra;
using MongoDB.Driver;

namespace GestaoPedidosPayment.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IMongoDatabase _database;
        private readonly IClientSessionHandle _session;
        private bool _disposed = false;

        public UnitOfWork(IMongoClient client, string databaseName)
        {
            _session = client.StartSession();
            _session.StartTransaction();
            _database = _session.Client.GetDatabase(databaseName);
        }

        public IRepository<T> GetRepository<T>() where T : BaseEntity
        {
            return new MongoRepository<T>(_database, typeof(T).Name);
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                await _session.CommitTransactionAsync();
                return true;
            }
            catch
            {
                Rollback();
                return false;
            }
        }

        public void Rollback()
        {
            _session.AbortTransaction();
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
                if (disposing)
                {
                    _session.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
