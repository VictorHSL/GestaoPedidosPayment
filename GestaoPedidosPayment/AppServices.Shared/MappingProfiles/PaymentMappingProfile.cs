using AutoMapper;
using GestaoPedidosPayment.AppServices.Shared.Dtos;
using GestaoPedidosPayment.Core.Entities;

namespace GestaoPedidosPayment.AppServices.Shared.MappingProfiles
{
    public class PaymentMappingProfile : Profile
    {
        public PaymentMappingProfile()
        {
            CreateMap<Payment, PaymentDto>();
            CreateMap<CreatePaymentDto, Payment>();
        }
    }
}
