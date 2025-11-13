using ArtemisBanking.Core.Application.Dtos.Beneficiary;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.DtoToViewModelMapping
{
    public class BeneficiaryMappingProfile : Profile
    {
        public BeneficiaryMappingProfile()
        {
            CreateMap<Beneficiary, BeneficiaryDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
        }
    }
}

