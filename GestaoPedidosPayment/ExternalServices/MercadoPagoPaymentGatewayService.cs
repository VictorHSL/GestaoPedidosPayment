namespace GestaoPedidosPayment.ExternalServices
{
    public class MercadoPagoPaymentGatewayService : IPaymentGatewayService
    {
        private readonly Dictionary<string, string> _paymentDatabase = new();

        public async Task<string> CreatePaymentAsync(decimal amount, string id)
        {
            string paymentId = Guid.NewGuid().ToString();
            _paymentDatabase[paymentId] = "pending";

            Console.WriteLine($"[Fake MercadoPago] Created payment {paymentId}: {amount}, {id}");
            return await Task.FromResult(paymentId);
        }

        public async Task<bool> CancelPaymentAsync(string paymentId)
        {
            if (_paymentDatabase.ContainsKey(paymentId))
            {
                _paymentDatabase[paymentId] = "canceled";
                Console.WriteLine($"[Fake MercadoPago] Canceled payment {paymentId}");
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<string> GetPaymentStatusAsync(string paymentId)
        {
            if (_paymentDatabase.TryGetValue(paymentId, out var status))
            {
                Console.WriteLine($"[Fake MercadoPago] Payment {paymentId} status: {status}");
                return await Task.FromResult(status);
            }
            return await Task.FromResult("not_found");
        }
    }
}
