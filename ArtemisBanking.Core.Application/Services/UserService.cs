using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.User;
using ArtemisBanking.Core.Application.Interfaces;
using AutoMapper;
using System.Text;

namespace ArtemisBanking.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IAccountServiceForApp _accountServiceForApp;
        private readonly IMapper _mapper;

        public UserService(IAccountServiceForApp accountServiceForApp, IMapper mapper)
        {
            _accountServiceForApp = accountServiceForApp;
            _mapper = mapper;
        }

        public async Task<ResultDto<List<UserDto>>> AllUser(int pagNum, int pagSize = 20)
        {
            var result = new ResultDto<List<UserDto>>();

            try
            {
                var users = await _accountServiceForApp.GetAllUser();
                var activeUsers = users.Where(u => u.Role != "Merchant").Skip((pagNum - 1) * pagSize).Take(pagSize).ToList();

                if (!activeUsers.Any())
                {
                    result.IsError = true;
                    result.Message = "No se encontraron usuarios activos.";
                    return result;
                }
                result.IsError = false;
                result.Result = _mapper.Map<List<UserDto>>(activeUsers);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<List<UserDto>>> FilterClientForRole(string role, int pagNum, int pagSize = 20)
        {
            var result = new ResultDto<List<UserDto>>();

            try
            {
                var users = await _accountServiceForApp.GetAllUser();
                var activeUsersForRole = users.Where(u => u.Role != "Merchant" && u.Role == role).Skip((pagNum - 1) * pagSize).Take(pagSize).ToList();

                if (!activeUsersForRole.Any())
                {
                    result.IsError = true;
                    result.Message = $"No se encontraron usuarios {role}.";
                    return result;
                }
                result.IsError = false;
                result.Result = _mapper.Map<List<UserDto>>(activeUsersForRole);
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<bool> IsActivatedUser(string userId)
        {
            try
            {
                var user = await _accountServiceForApp.GetUserById(userId);
                if (user == null)
                {
                    return false;
                }
                user.IsActive = !user.IsActive;
                await _accountServiceForApp.SetActivated(user);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
