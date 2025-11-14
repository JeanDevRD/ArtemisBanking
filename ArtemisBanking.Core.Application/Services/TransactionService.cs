using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Email;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;



namespace ArtemisBanking.Core.Application.Services
{
    public class TransactionService : GenericService<TransactionDto, Transaction>, ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISavingsAccountRepository _savingsAccountRepository;
        private readonly IAccountServiceForApp _accountServiceForApp;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public TransactionService(ITransactionRepository transactionRepository, IEmailService emailService,IMapper mapper, ISavingsAccountRepository savingsAccountRepository, IAccountServiceForApp accountServiceForApp): base(transactionRepository , mapper)
        {
            _transactionRepository = transactionRepository;
            _savingsAccountRepository = savingsAccountRepository;
            _accountServiceForApp = accountServiceForApp;
            _emailService = emailService;
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

        public async Task<ResultDto<TransactionConfirmDto>> ValidateTransactionAsync(DepositTransactionDto dto, bool IsWithdrawal = false)
        {
            var result = new ResultDto<TransactionConfirmDto>();

            try
            {
                var account = await _savingsAccountRepository.GetAllQueryAsync().Where(x => x.AccountNumber == dto.AccountNumber).FirstOrDefaultAsync();
                var client = await _accountServiceForApp.GetUserById(account!.AssignedByUserId!);
                
                if(client == null)
                {
                    result.IsError = true;
                    result.Message = "El usuario asignado a la cuenta no es válido";
                    return result;
                }

                var fullname = $"{client.FirstName} {client.LastName}";

                if (account == null || !account.IsActive)
                {
                    result.IsError = true;
                    result.Message = "El número de cuenta ingresado no es válido o la cuenta es inactiva";
                    return result;
                }

                if(IsWithdrawal && account.Balance < dto.Amount)
                {
                    result.IsError = true;
                    result.Message = "Fondos insuficientes para completar el retiro";
                    return result;
                }

                result.IsError = false;
                result.Result = new TransactionConfirmDto
                {
                    AccountNumber = dto.AccountNumber,
                    Amount = dto.Amount,
                    HolderName = fullname
                };
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<bool> ConfirmDepositAsync(DepositTransactionDto dto, string crashierId)
        {
            try
            {
                var account = await _savingsAccountRepository.GetAllQueryAsync().Where(x => x.AccountNumber == dto.AccountNumber).FirstOrDefaultAsync();
                var client = await _accountServiceForApp.GetUserById(account!.AssignedByUserId!);
                account!.Balance += dto.Amount;
                await _savingsAccountRepository.UpdateAsync(account.Id, account);

                var transaction = new Transaction
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    Amount = dto.Amount,
                    Type = (int)TypeCard.Debit,
                    TypeTransaction = (int)TypeTransaction.Deposit,
                    Status = (int)StatusTransaction.Approved,
                    SavingsAccountId = account.Id,
                    CashierId = crashierId,
                    Source = "Cashier Deposit",
                    Beneficiary = account.AccountNumber

                };

                await _transactionRepository.AddAsync(transaction);

                string lastnumber = account.AccountNumber.Substring(account.AccountNumber.Length - 4);

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = client!.Email,
                    Subject = $"Depósito",
                    HtmlBody = $@"
                        <h3>Estimado {client.FirstName} {client.LastName},</h3>
                        <p>Se ha realizado un depósito a su cuenta xxx-xxx-{lastnumber}.</p>
                        <p>Monto: {dto.Amount:C}<br/>
                         Fecha: {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}</p>
                        <p>Gracias por confiar en Artemis Banking.</p>"
                });

                return true;

            }
            catch (Exception ex)
            {
               return false;
            }

        }

        public async Task<bool> ConfirmWithdrawalAsync(WithdrawalTransactionDto dto, string crashierId) 
        {
            try
            {
                var account = await _savingsAccountRepository.GetAllQueryAsync().Where(x => x.AccountNumber == dto.AccountNumber).FirstOrDefaultAsync();
                var client = await _accountServiceForApp.GetUserById(account!.AssignedByUserId!);
                account!.Balance -= dto.Amount;
                await _savingsAccountRepository.UpdateAsync(account.Id, account);

                var transaction = new Transaction
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    Amount = dto.Amount,
                    Type = (int)TypeCard.Debit,
                    TypeTransaction = (int)TypeTransaction.Deposit,
                    Status = (int)StatusTransaction.Approved,
                    SavingsAccountId = account.Id,
                    CashierId = crashierId,
                    Source = "Withdrawal Deposit",
                    Beneficiary = account.AccountNumber

                };

                await _transactionRepository.AddAsync(transaction);

                string lastnumber = account.AccountNumber.Substring(account.AccountNumber.Length - 4);

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = client!.Email,
                    Subject = $"Retiro",
                    HtmlBody = $@"
                        <h3>Estimado {client.FirstName} {client.LastName},</h3>
                        <p>Se ha realizado un retro a su cuenta xxx-xxx-{lastnumber}.</p>
                        <p>Monto: {dto.Amount:C}<br/>
                         Fecha: {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}</p>
                        <p>Gracias por confiar en Artemis Banking.</p>"
                });

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }
    }
}
