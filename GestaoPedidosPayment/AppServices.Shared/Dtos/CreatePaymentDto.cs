namespace GestaoPedidosPayment.AppServices.Shared.Dtos
{
    public class CreatePaymentDto
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
    }
}
