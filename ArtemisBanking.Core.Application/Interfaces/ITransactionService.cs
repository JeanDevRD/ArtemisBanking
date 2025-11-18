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
        Task<ResultDto<TransactionConfirmDto>> ValidateCreditCardPaymentAsync(string accountNumber, string cardNumber,
            decimal amount);
        Task<bool> ConfirmCreditCardPaymentAsync(string accountNumber, string cardNumber, decimal amount, string cashierId);
        Task<ResultDto<TransactionConfirmDto>> ValidateLoanPaymentAsync(string accountNumber, string loanNumber, decimal amount);
        Task<bool> ConfirmLoanPaymentAsync(string accountNumber, string loanNumber, decimal amount, string cashierId);
        Task<ResultDto<TransactionConfirmDto>> ValidateThirdPartyTransactionAsync(string accountOrigin,
            string accountDestination, decimal amount);
        Task<bool> ConfirmThirdPartyTransactionAsync(string accountOrigin, string accountDestination, 
            decimal amount, string cashierId);
    }
}
