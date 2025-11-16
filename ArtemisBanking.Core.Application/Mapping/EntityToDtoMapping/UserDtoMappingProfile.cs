using ArtemisBanking.Core.Application.Dtos.Loan;
using ArtemisBanking.Core.Application.Dtos.User;
using AutoMapper;


namespace ArtemisBanking.Core.Application.Mapping.EntityToDtoMapping
{
    public class UserDtoMappingProfile : Profile
    {
                public UserDtoMappingProfile()
                {
                      CreateMap<UserDto, ElegibleUserForLoanDto>()
                     .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                     .ForMember(dest => dest.MonthlyIncome, opt => opt.MapFrom(src => 0m))
                     .ReverseMap();

                }
    }
}
