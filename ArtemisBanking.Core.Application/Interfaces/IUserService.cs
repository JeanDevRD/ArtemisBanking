using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.User;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface IUserService
    {
        Task<ResultDto<List<UserDto>>> AllUser(int pagNum, int pagSize = 20);
        Task<ResultDto<List<UserDto>>> FilterClientForRole(string role, int pagNum, int pagSize = 20);

        Task<bool> IsActivatedUser(string userId);
    }
}