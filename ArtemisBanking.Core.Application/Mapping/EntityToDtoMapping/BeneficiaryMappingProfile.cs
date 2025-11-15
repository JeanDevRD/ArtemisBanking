
using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.DtoToViewModelMapping.EntityToDtoMapping
{
    public class BeneficiaryMappingProfile : Profile
    {
        public BeneficiaryMappingProfile()
        {
            CreateMap<Beneficiary, BeneficiaryDto>()
                .ForMember(dest => dest.AccountTypeName, opt => opt.MapFrom(src => src.AccountType.ToString()));
        }
    }
}