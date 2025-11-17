using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Loan;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface ILoanService : IGenericService<LoanDto>
    {
        Task<List<LoanDto>> GetAllWithInclude();
        Task<ResultDto<LoanDto>> AddLoanAsync(CreateLoanRequestDto dto);
        Task<ResultDto<List<ElegibleUserForLoanDto>>> GetElegibleUserForLoan();
        Task<ResultDto<ElegibleUserForLoanDto>> GetElegibleUserByIdentityForLoan(string identificationNumber);
        Task<ResultDto<List<LoanListDto>>?> GetAllActiveLoanAsync();
        Task<ResultDto<List<LoanListDto>>> GetLoansByUserIdentity(string identificationNumber, bool isActive);
        Task<ResultDto<LoanDetailDto>> GetLoanDetailAsync(int loanId);
        Task<ResultDto<LoanDto>> UpdateInterestRateAsync(int loanId, decimal newAnnualInterestRate);
        Task<ResultDto<List<LoanListDto>>> GetAllLoansAsync(bool? isActive = null);
    }
}
