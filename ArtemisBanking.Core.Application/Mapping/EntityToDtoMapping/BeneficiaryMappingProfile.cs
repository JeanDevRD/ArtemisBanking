using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;
namespace ArtemisBanking.Core.Application.Mapping.EntityToDtoMapping
{
    public class BeneficiaryMappingProfile : Profile
    {
        public BeneficiaryMappingProfile()
        {
         
             CreateMap<Beneficiary, BeneficiaryDto>()
                .ReverseMap();
        }
    }
}
