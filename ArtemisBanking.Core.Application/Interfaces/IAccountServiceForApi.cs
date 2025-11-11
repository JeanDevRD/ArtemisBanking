using ArtemisBanking.Core.Application.Dtos.User;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface IAccountServiceForApi : IBaseAccountService
    {
        Task<LoginResponseForAPiDto> AuthenticateAsync(LoginDto loginDto);
    }
}
