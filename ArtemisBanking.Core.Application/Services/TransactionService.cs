using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;


namespace ArtemisBanking.Core.Application.Services
{
    public class TransactionService : GenericService<TransactionDto, Transaction>, ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        public TransactionService(ITransactionRepository transactionRepository, IMapper mapper): base(transactionRepository , mapper)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
        }
        public async Task<List<TransactionDto>> GetAllWithInclude()
        {
            try
            {
                var creditCards = await _transactionRepository.GetAllListIncluideAsync(["Transactions"]);
                if (creditCards == null)
                {
                    return new List<TransactionDto>();
                }
                return _mapper.Map<List<TransactionDto>>(creditCards);
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving credit cards with included data: " + ex.Message);
            }
        }

        public async Task<ResultDto<CountTransactionDto>> CountTransactionsByCasherLog(string IdCasher) 
        { 
            var result = new ResultDto<CountTransactionDto>();
            try 
            { 
                var count = await _transactionRepository.GetAllQueryAsync().Where(t => t.CashierId == IdCasher).CountAsync();
                
                if (count == 0) 
                {
                    result.IsError = true;
                    result.Message = "Aun no hay transacciones realizadas por este cajero";
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
                var count = await _transactionRepository.GetAllQueryAsync().Where(t => t.CashierId == IdCasher && t.TypeTransaction == (int)TypeTransaction.Deposit).CountAsync();

                if (count == 0)
                {
                    result.IsError = true;
                    result.Message = "Aun no hay depositos realizadas por este cajero";
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
                    result.Message = "Aun no hay retiros realizadas por este cajero";
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


    }
}
