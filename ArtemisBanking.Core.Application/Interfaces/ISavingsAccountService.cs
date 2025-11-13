using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.SavingsAccount;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface ISavingsAccountService : IGenericService<SavingsAccountDto>
    {
        Task<List<SavingsAccountDto>> GetAllWithInclude();
        Task AddBalance(string userId, decimal amount);
        Task<ResultDto<List<SavingsAccountsHomeDto>>> GetSavingAccountHome(string? identificationNumber, int page, bool? isActive = null, int? accountType = null);
    }
}
