using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.User;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResultDto<List<UserDto>>> AllUserActive(int pagNum, int pagSize = 20);
    }
}