using GestaoPedidosPayment.Repositories;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace Tests.Repositories
{
    public class BaseMongoRepositoryTests : IDisposable
    {
        private readonly MongoDbRunner _mongoRunner;
        private readonly IMongoDatabase _database;
        private readonly BaseMongoRepository<TestEntity> _repository;

        public BaseMongoRepositoryTests()
        {
            _mongoRunner = MongoDbRunner.Start();
            var client = new MongoClient(_mongoRunner.ConnectionString);
            _database = client.GetDatabase("TestDatabase");
            _repository = new BaseMongoRepository<TestEntity>(_database, "TestCollection");
        }

        [Fact(DisplayName = "AddAsync should insert entity into collection")]
        public async Task AddAsync_ShouldInsertEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };

            // Act
            await _repository.AddAsync(entity);
            var result = await _repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal(entity.Name, result.Name);
        }

        [Fact(DisplayName = "GetByIdAsync should return correct entity")]
        public async Task GetByIdAsync_ShouldReturnCorrectEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };
            await _repository.AddAsync(entity);

            // Act
            var result = await _repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact(DisplayName = "FindOneAsync should return matching entity")]
        public async Task FindOneAsync_ShouldReturnMatchingEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };
            await _repository.AddAsync(entity);

            // Act
            var result = await _repository.FindOneAsync(x => x.Id == entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact(DisplayName = "GetAllAsync should return all entities")]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            var entity1 = new TestEntity { Id = Guid.NewGuid(), Name = "Test1" };
            var entity2 = new TestEntity { Id = Guid.NewGuid(), Name = "Test2" };

            await _repository.AddAsync(entity1);
            await _repository.AddAsync(entity2);

            // Act
            var results = await _repository.GetAllAsync();

            // Assert
            Assert.Equal(2, results.Count);
        }

        [Fact(DisplayName = "UpdateAsync should modify existing entity")]
        public async Task UpdateAsync_ShouldModifyEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Old Name" };
            await _repository.AddAsync(entity);

            entity.Name = "New Name";
            await _repository.UpdateAsync(entity);

            // Act
            var updatedEntity = await _repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(updatedEntity);
            Assert.Equal("New Name", updatedEntity.Name);
        }

        [Fact(DisplayName = "DeleteAsync should remove entity from collection")]
        public async Task DeleteAsync_ShouldRemoveEntity()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test" };
            await _repository.AddAsync(entity);

            // Act
            await _repository.DeleteAsync(entity.Id);
            var deletedEntity = await _repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.Null(deletedEntity);
        }

        public void Dispose()
        {
            _mongoRunner.Dispose();
        }
    }
}
