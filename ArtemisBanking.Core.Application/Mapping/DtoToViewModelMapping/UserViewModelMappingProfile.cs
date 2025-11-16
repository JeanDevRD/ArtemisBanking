using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.ViewModels.User;
using AutoMapper;

namespace ArtemisBanking.Core.Application.Mapping.DtoToViewModelMapping
{
    public class UserViewModelMappingProfile : Profile
    {
        public UserViewModelMappingProfile()
        {
            CreateMap<UserDto, UserViewModel>()
                .ReverseMap();
            
            CreateMap<UserDto, SaveUserDto>()
                .ReverseMap();
            
            CreateMap<SaveUserDto, SaveUserViewModel>()
                .ReverseMap();
            
            CreateMap<SaveUserDto, EditUserViewModel>()
                .ReverseMap();
        }
    }
}

