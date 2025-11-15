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

        public async Task<ResultDto<List<UserDto>>> AllUserActive(int pagNum, int pagSize = 20)
        {
            var result = new ResultDto<List<UserDto>>();

            try
            {
                var users = await _accountServiceForApp.GetAllUser();
                var activeUsers = users.Where(u => u.IsActive && u.Role != "Merchant").Skip((pagNum - 1) * pagSize).Take(pagSize).ToList();

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
    }
}
