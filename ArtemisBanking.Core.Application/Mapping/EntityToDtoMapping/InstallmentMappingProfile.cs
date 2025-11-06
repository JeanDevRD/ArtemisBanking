using ArtemisBanking.Core.Application.Dtos.Installment;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;
namespace ArtemisBanking.Core.Application.Mapping.EntityToDtoMapping
{
    public class InstallmentMappingProfile : Profile
    {
        public InstallmentMappingProfile() 
        { 
          CreateMap<Installment,InstallmentDto>()
                .ReverseMap();
        }
    }
}
