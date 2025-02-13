using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace GestaoPedidosPayment.Core.Entities
{
    public abstract class BaseEntity
    {
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
