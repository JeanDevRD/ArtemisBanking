using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Transaction;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface ITransactionService : IGenericService<TransactionDto>
    {
        Task<List<TransactionDto>> GetAllWithInclude();
        Task<ResultDto<TransactionConfirmDto>> ValidateTransactionAsync(DepositTransactionDto dto, bool IsWithdrawal = false);
        Task<bool> ConfirmDepositAsync(DepositTransactionDto dto, string crashierId);
        Task<bool> ConfirmWithdrawalAsync(WithdrawalTransactionDto dto, string crashierId);
    }
}
