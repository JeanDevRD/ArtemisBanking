using ArtemisBanking.Core.Application.Dtos.Merchant;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.EntityToDtoMapping
{
    public class MerchantMappingProfile : Profile
    { 
        public MerchantMappingProfile()
        {
             CreateMap<Merchant, MerchandDto>()
                .ReverseMap();
        }
    }
}
