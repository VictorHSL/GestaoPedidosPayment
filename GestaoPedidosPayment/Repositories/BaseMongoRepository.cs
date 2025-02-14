using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace GestaoPedidosPayment.Repositories
{
    public class BaseMongoRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly IMongoCollection<T> _collection;

        public BaseMongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await _collection.Find(predicate).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.AsQueryable().ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            await _collection.ReplaceOneAsync(filter, entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}
