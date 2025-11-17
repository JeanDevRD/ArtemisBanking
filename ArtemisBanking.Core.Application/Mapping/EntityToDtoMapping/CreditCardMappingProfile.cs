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

            CreateMap<CreditCard, CreditCardListDto>()
                .ForMember(dest => dest.FullNameUser, opt => opt.Ignore());

            CreateMap<CreditCard, CreditCardDetailDto>()
                .ForMember(dest => dest.cardId, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.cardNumberMasked, opt => opt.MapFrom(src =>
                $"****-****-****-{src.CardNumber.Substring(src.CardNumber.Length - 4)}"
                ))
                .ForMember(dest => dest.consumptions, opt => opt.Ignore());
        }
    }
}
