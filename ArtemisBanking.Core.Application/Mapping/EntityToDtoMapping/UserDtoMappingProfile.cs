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

            CreateMap<UserDto, ElegibleUserForCreditCardDto>()
              .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.IdentificationNumber, opt => opt.MapFrom(src => src.IdentificationNumber))
              .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
              .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
              .ForMember(dest => dest.MonthlyIncome, opt => opt.Ignore());
        } 
    }
}
