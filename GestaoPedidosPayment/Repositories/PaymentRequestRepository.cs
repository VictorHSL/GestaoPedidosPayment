using GestaoPedidosPayment.Core.Entities;
using GestaoPedidosPayment.Core.Repositories;
using MongoDB.Driver;

namespace GestaoPedidosPayment.Repositories
{
    public class PaymentRequestRepository : MongoRepository<PaymentRequest>, IPaymentRequestRepository
    {
        private const string CollectionName = "PaymentRequests";
        public PaymentRequestRepository(IMongoDatabase database): base(database, CollectionName)
        {
        }
    }
}
