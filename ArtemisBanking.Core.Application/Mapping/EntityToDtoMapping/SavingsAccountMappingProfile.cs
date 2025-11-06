using ArtemisBanking.Core.Application.Dtos.SavingsAccount;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;
namespace ArtemisBanking.Core.Application.Mapping.EntityToDtoMapping
{
    public class SavingsAccountMappingProfile : Profile
    {
        public SavingsAccountMappingProfile()
        {
            CreateMap<SavingsAccount, SavingsAccountDto>()
                .ReverseMap();
        }
    }
}
