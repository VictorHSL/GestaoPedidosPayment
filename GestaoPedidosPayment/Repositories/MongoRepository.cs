using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Repositories;
using MongoDB.Driver;

namespace GestaoPedidosPayment.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
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
