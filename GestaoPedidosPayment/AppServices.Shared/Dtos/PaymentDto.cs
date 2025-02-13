using GestaoPedidosPayment.Core.Shared.ValueObjects;

namespace GestaoPedidosPayment.AppServices.Shared.Dtos
{
    public class PaymentDto : BaseEntityDto
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
    }
}
