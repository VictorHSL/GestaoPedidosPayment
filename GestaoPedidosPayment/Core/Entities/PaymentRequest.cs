using GestaoPedidosPayment.Core.Shared.ValueObjects;

namespace GestaoPedidosPayment.Core.Entities
{
    public class PaymentRequest : BaseEntity
    {
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
