using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Repositories;
using MongoDB.Driver;

namespace GestaoPedidosPayment.Repositories
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        private const string CollectionName = "PaymentRequests";
        public PaymentRepository(IMongoDatabase database): base(database, CollectionName)
        {

        }

        public async Task<Payment> GetByOrderIdAsync(string orderId)
        {
            var payment = await FindOneAsync(p => p.OrderId == orderId);
            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with OrderId '{orderId}' was not found.");
            }
            return payment;
        }
    }
}
