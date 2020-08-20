using AutoMapper;
using Panier.Contracts.V1.Requests;
using Panier.Domain.Entities;


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
