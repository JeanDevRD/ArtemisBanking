using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services
{
    public class CasherDashboardService : ICasherDashboardService
    {
        public readonly ITransactionRepository _transactionRepository;
        public CasherDashboardService(ITransactionRepository transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<ResultDto<CountTransactionDto>> CountTransactionsByCasherLog(string IdCasher)
        {
            var result = new ResultDto<CountTransactionDto>();
            try
            {
                var count = await _transactionRepository.GetAllQueryAsync().Where(t => t.CashierId == IdCasher && t.Date.Date == DateTime.Today).CountAsync();

                if (count == 0)
                {
                    result.IsError = true;
                    result.Message = "Aun no hay transacciones realizadas por este cajero en el dia de hoy";
                    return result;
                }

                result.Result = new CountTransactionDto
                {
                    TotalCountTransaction = count
                };
            }
            catch (Exception ex)
            {
                result.IsError = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultDto<CountDepositDto>> CountDepositByCasherLog(string IdCasher)
        {
            var result = new ResultDto<CountDepositDto>();
            try
            {
                var count = await _transactionRepository.GetAllQueryAsync().Where(t => t.CashierId == IdCasher && t.TypeTransaction == (int)TypeTransaction.Deposit && t.Date.Date == DateTime.Today).CountAsync();

                if (count == 0)
                {
                    result.IsError = true;
                    result.Message = "Aun no hay depositos realizadas por este cajero en el dia de hoy";
                    return result;
                }

                result.Result = new CountDepositDto
                {
                    TotalCountDeposit = count
                };
            }
            catch (Exception ex)
            {
                result.IsError = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultDto<CountWithdrawalDto>> CountWithdrawaltByCasherLog(string IdCasher)
        {
            var result = new ResultDto<CountWithdrawalDto>();
            try
            {
                var count = await _transactionRepository.GetAllQueryAsync().Where(t => t.CashierId == IdCasher && t.TypeTransaction == (int)TypeTransaction.Withdrawal).CountAsync();

                if (count == 0)
                {
                    result.IsError = true;
                    result.Message = "Aun no hay retiros realizadas por este cajero en el dia de hoy";
                    return result;
                }

                result.Result = new CountWithdrawalDto
                {
                    TotalCountWithdrawal = count
                };
            }
            catch (Exception ex)
            {
                result.IsError = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultDto<CountPaidsDto>> CountPaidsByCasherLog(string IdCasher)
        {
            var result = new ResultDto<CountPaidsDto>();
            try
            
            {
                var transctions = await _transactionRepository.GetAllListAsync();

                var paids = transctions.Where(t => t.CashierId == IdCasher && (t.TypeTransaction == (int)TypeTransaction.LoanPaid || t.TypeTransaction == (int)TypeTransaction.CreditCardPaid) && t.Date.Date == DateTime.Today);

                var count = paids.Count();

                if (count == 0)
                {
                    result.IsError = true;
                    result.Message = "Aun no hay pagos realizadas por este cajero en el dia de hoy";
                    return result;
                }

                result.Result = new CountPaidsDto
                {
                    TotalCountPaids = count
                };
            }
            catch (Exception ex)
            {
                result.IsError = false;
                result.Message = ex.Message;
            }
            return result;
        }


    }
}
