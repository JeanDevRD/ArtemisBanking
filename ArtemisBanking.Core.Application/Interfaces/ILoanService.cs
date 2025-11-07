

using ArtemisBanking.Core.Application.Dtos.Loan;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface ILoanService : IGenericService<LoanDto>
    {
        Task<List<LoanDto>> GetAllWithInclude();
    }
}
