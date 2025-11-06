using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Domain.Entities;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.EntityToDtoMapping
{
    public class LoanMappingProfile : Profile
    {
        public LoanMappingProfile()
        {
            CreateMap<Loan, LoanDto>()
                .ReverseMap();
        }
    }
}
