using AutoMapper;
using Panier.Business.Contracts.V1.Requests;
using Panier.Entities;

namespace Panier.Porfiles
{
    public class RequestToDomainProfile : Profile
    {
        public RequestToDomainProfile()
        {
            CreateMap<AddToBasketRequestModel, BasketItem>().ReverseMap();
        }
    }
}
