namespace GestaoPedidosPayment.ExternalServices
{
    public interface IPaymentGatewayService
    {
        Task<string> CreatePaymentAsync(decimal amount, string id);
        Task<bool> CancelPaymentAsync(string paymentId);
        Task<string> GetPaymentStatusAsync(string paymentId);
    }
}
