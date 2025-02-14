using AutoMapper;
using GestaoPedidosPayment.AppServices.Shared;
using GestaoPedidosPayment.AppServices.Shared.Dtos;
using GestaoPedidosPayment.Core.Managers.Interfaces;

namespace GestaoPedidosPayment.AppServices
{
    public class PaymentAppService : IPaymentAppService
    {
        private readonly IPaymentManager _paymentManager;
        private readonly IMapper _mapper;

        public PaymentAppService(IPaymentManager paymentManager, IMapper mapper)
        {
            _paymentManager = paymentManager;
            _mapper = mapper;
        }

        public async Task<PaymentDto> CreatePayment(CreatePaymentDto createPaymentDto)
        {
            var payment = await _paymentManager.CreatePayment(createPaymentDto.OrderId, createPaymentDto.Amount);
            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> GetPayment(Guid id)
        {
            var payment = await _paymentManager.GetPayment(id);
            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task<PaymentDto> GetPayment(string orderId)
        {
            var payment = await _paymentManager.GetPayment(orderId);
            return _mapper.Map<PaymentDto>(payment);
        }

        public async Task CancelPayment(Guid id, string? reason = null)
        {
            await _paymentManager.CancelPayment(id, reason);
        }

        public async Task CancelPayment(string orderId, string? reason = null)
        {
            await _paymentManager.CancelPayment(orderId, reason);
        }

        public async Task SetCompleted(Guid id)
        {
            await _paymentManager.SetCompleted(id);
        }
    }
}
