using ArtemisBanking.Core.Application.Dtos.SavingsAccount;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface ISavingsAccountService : IGenericService<SavingsAccountDto>
    {
        Task<List<SavingsAccountDto>> GetAllWithInclude();
        Task AddBalance(string userId, decimal amount);
    }
}
