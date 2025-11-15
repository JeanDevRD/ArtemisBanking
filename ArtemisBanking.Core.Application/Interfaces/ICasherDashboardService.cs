using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Transaction;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface ICasherDashboardService
    {
        Task<ResultDto<CountDepositDto>> CountDepositByCasherLog(string IdCasher);
        Task<ResultDto<CountPaidsDto>> CountPaidsByCasherLog(string IdCasher);
        Task<ResultDto<CountTransactionDto>> CountTransactionsByCasherLog(string IdCasher);
        Task<ResultDto<CountWithdrawalDto>> CountWithdrawaltByCasherLog(string IdCasher);
    }
}