using GestaoPedidosPayment.Core.Shared.ValueObjects;

namespace GestaoPedidosPayment.Core.Entities
{
    public class Payment : BaseEntity
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string? Comments { get; set; }

        public void SetPaymentId(string paymentId)
        {
            PaymentId = paymentId;
        }
    }
}
