using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Repositories;
using Mongo2Go;
using MongoDB.Driver;
using Xunit;

namespace Tests.Repositories
{
    public class PaymentMongoRepositoryTests : IDisposable
    {
        private readonly MongoDbRunner _mongoRunner;
        private readonly IMongoDatabase _database;
        private readonly PaymentMongoRepository _repository;

        public PaymentMongoRepositoryTests()
        {
            _mongoRunner = MongoDbRunner.Start();
            var client = new MongoClient(_mongoRunner.ConnectionString);
            _database = client.GetDatabase("TestDatabase");
            _repository = new PaymentMongoRepository(_database);
        }

        [Fact(DisplayName = "AddAsync should insert a payment into collection")]
        public async Task AddAsync_ShouldInsertPayment()
        {
            // Arrange
            var payment = new Payment { Id = Guid.NewGuid(), OrderId = "ORDER123", Amount = 100.0m };

            // Act
            await _repository.AddAsync(payment);
            var result = await _repository.GetByIdAsync(payment.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(payment.Id, result.Id);
            Assert.Equal(payment.OrderId, result.OrderId);
        }

        [Fact(DisplayName = "GetByOrderIdAsync should return correct payment")]
        public async Task GetByOrderIdAsync_ShouldReturnCorrectPayment()
        {
            // Arrange
            var payment = new Payment { Id = Guid.NewGuid(), OrderId = "ORDER123", Amount = 200.0m };
            await _repository.AddAsync(payment);

            // Act
            var result = await _repository.GetByOrderIdAsync(payment.OrderId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(payment.OrderId, result.OrderId);
        }

        [Fact(DisplayName = "GetByOrderIdAsync should throw KeyNotFoundException if not found")]
        public async Task GetByOrderIdAsync_ShouldThrowKeyNotFoundException_IfNotFound()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _repository.GetByOrderIdAsync("INVALID_ORDER"));
            Assert.Contains("Payment with OrderId 'INVALID_ORDER' was not found.", exception.Message);
        }

        [Fact(DisplayName = "DeleteAsync should remove payment from collection")]
        public async Task DeleteAsync_ShouldRemovePayment()
        {
            // Arrange
            var payment = new Payment { Id = Guid.NewGuid(), OrderId = "ORDER123", Amount = 150.0m };
            await _repository.AddAsync(payment);

            // Act
            await _repository.DeleteAsync(payment.Id);
            var deletedPayment = await _repository.GetByIdAsync(payment.Id);

            // Assert
            Assert.Null(deletedPayment);
        }

        public void Dispose()
        {
            _mongoRunner.Dispose();
        }
    }
}
