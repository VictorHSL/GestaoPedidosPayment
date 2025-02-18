using GestaoPedidosPayment.Repositories;
using MongoDB.Driver;
using NSubstitute;
using Xunit;

namespace Tests.Repositories
{
    public class UnitOfWorkTests
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly UnitOfWork _unitOfWork;

        public UnitOfWorkTests()
        {
            _mongoClient = Substitute.For<IMongoClient>();
            _database = Substitute.For<IMongoDatabase>();

            _mongoClient.GetDatabase(Arg.Any<string>()).Returns(_database);
            _unitOfWork = new UnitOfWork(_mongoClient, "TestDatabase");
        }

        [Fact(DisplayName = "Should retrieve repository instance of correct type")]
        public void GetRepository_ShouldReturnCorrectRepositoryInstance()
        {
            // Act
            var repository = _unitOfWork.GetRepository<TestEntity>();

            // Assert
            Assert.NotNull(repository);
            Assert.IsType<BaseMongoRepository<TestEntity>>(repository);
        }

        [Fact(DisplayName = "Should return the same repository instance for the same entity type")]
        public void GetRepository_ShouldReturnSameInstanceForSameType()
        {
            // Act
            var repository1 = _unitOfWork.GetRepository<TestEntity>();
            var repository2 = _unitOfWork.GetRepository<TestEntity>();

            // Assert
            Assert.Same(repository1, repository2);
        }

        [Fact(DisplayName = "Should mark unit of work as disposed when disposed")]
        public void Dispose_ShouldMarkUnitOfWorkAsDisposed()
        {
            // Act
            _unitOfWork.Dispose();

            // Assert
            _unitOfWork.Dispose(); // Call Dispose again to ensure it doesn't throw an error
        }
    }
}
