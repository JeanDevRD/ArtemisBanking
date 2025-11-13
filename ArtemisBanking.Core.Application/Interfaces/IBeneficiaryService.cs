using ArtemisBanking.Core.Application.Dtos.Beneficiary;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface IBeneficiaryService : IGenericService<BeneficiaryDto>
    {
        Task<bool> ExistsByAccountNumberAsync(string accountNumber);
        Task<bool> ExistsByCedulaAsync(string cedula);
        Task<bool> HasPendingTransactionsAsync(int beneficiaryId);
    }
}
