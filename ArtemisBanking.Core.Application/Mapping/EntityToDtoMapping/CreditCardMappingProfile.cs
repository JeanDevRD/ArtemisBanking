using ArtemisBanking.Core.Application.Dtos.CreditCard;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;
namespace ArtemisBanking.Core.Application.Mapping.EntityToDtoMapping
{
    public class CreditCardMappingProfile : Profile
    {
        public CreditCardMappingProfile()
        {
            CreateMap<CreditCard, CreditCardDto>()
                .ReverseMap();
        }
    }
}
