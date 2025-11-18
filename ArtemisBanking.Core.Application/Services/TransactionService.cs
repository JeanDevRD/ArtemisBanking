using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Dtos.Email;
using ArtemisBanking.Core.Application.Dtos.Transaction;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Core.Domain.Entities;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace ArtemisBanking.Core.Application.Services
{
    public class TransactionService : GenericService<TransactionDto, Transaction>, ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ISavingsAccountRepository _savingsAccountRepository;
        private readonly IAccountServiceForApp _accountServiceForApp;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly ICreditCardRepository _creditCardRepository;
        private readonly ILoanRepository _loanRepository;
        public TransactionService(ITransactionRepository transactionRepository, IEmailService emailService,IMapper mapper, 
            ISavingsAccountRepository savingsAccountRepository,IAccountServiceForApp accountServiceForApp, 
            ICreditCardRepository creditCardRepository, ILoanRepository loanRepository): base(transactionRepository , mapper)
        {
            _transactionRepository = transactionRepository;
            _savingsAccountRepository = savingsAccountRepository;
            _accountServiceForApp = accountServiceForApp;
            _emailService = emailService;
            _mapper = mapper;
            _creditCardRepository = creditCardRepository;
            _loanRepository = loanRepository;
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
                var client = await _accountServiceForApp.GetUserById(account!.UserId);

                if (client == null)
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
                var client = await _accountServiceForApp.GetUserById(account!.UserId);
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
                var client = await _accountServiceForApp.GetUserById(account!.UserId);
                account!.Balance -= dto.Amount;
                await _savingsAccountRepository.UpdateAsync(account.Id, account);

                var transaction = new Transaction
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    Amount = dto.Amount,
                    Type = (int)TypeCard.Debit,
                    TypeTransaction = (int)TypeTransaction.Withdrawal,
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

        public async Task<ResultDto<TransactionConfirmDto>> ValidateCreditCardPaymentAsync(string accountNumber, 
            string cardNumber, decimal amount)
        {
            var result = new ResultDto<TransactionConfirmDto>();

            try
            {
                var account = await _savingsAccountRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);

                if (account == null || !account.IsActive)
                {
                    result.IsError = true;
                    result.Message = "El número de cuenta origen no es válido o está inactiva";
                    return result;
                }

                if (account.Balance < amount)
                {
                    result.IsError = true;
                    result.Message = "Fondos insuficientes en la cuenta origen";
                    return result;
                }

                var creditCard = await _creditCardRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(c => c.CardNumber == cardNumber);

                if (creditCard == null || !creditCard.IsActive)
                {
                    result.IsError = true;
                    result.Message = "El número de tarjeta ingresado no es válido o está inactiva";
                    return result;
                }

                var cardOwner = await _accountServiceForApp.GetUserById(creditCard.UserId);
                if (cardOwner == null)
                {
                    result.IsError = true;
                    result.Message = "Error al obtener información del titular de la tarjeta";
                    return result;
                }

                result.IsError = false;
                result.Result = new TransactionConfirmDto
                {
                    AccountId = account.Id,
                    AccountNumber = accountNumber,
                    HolderName = $"{cardOwner.FirstName} {cardOwner.LastName}",
                    Amount = amount,
                    CurrentDebt = creditCard.CurrentDebt
                };
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<bool> ConfirmCreditCardPaymentAsync(string accountNumber, string cardNumber, 
            decimal amount, string cashierId)
        {
            try
            {
                var account = await _savingsAccountRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

                var creditCard = await _creditCardRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(c => c.CardNumber == cardNumber);

                if (account == null || creditCard == null) return false;

                var actualPayment = Math.Min(amount, creditCard.CurrentDebt);

                account.Balance -= actualPayment;
                await _savingsAccountRepository.UpdateAsync(account.Id, account);

                creditCard.CurrentDebt -= actualPayment;
                await _creditCardRepository.UpdateAsync(creditCard.Id, creditCard);

                var transaction = new Transaction
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    Amount = actualPayment,
                    Type = (int)TypeCard.Debit,
                    TypeTransaction = (int)TypeTransaction.CreditCardPaid,
                    Status = (int)StatusTransaction.Approved,
                    SavingsAccountId = account.Id,
                    CashierId = cashierId,
                    Source = account.AccountNumber,
                    Beneficiary = creditCard.CardNumber
                };

                await _transactionRepository.AddAsync(transaction);

                var client = await _accountServiceForApp.GetUserById(creditCard.UserId);
                var lastFourAccount = account.AccountNumber.Substring(account.AccountNumber.Length - 4);
                var lastFourCard = creditCard.CardNumber.Substring(creditCard.CardNumber.Length - 4);

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = client!.Email,
                    Subject = $"Pago realizado a la tarjeta ****{lastFourCard}",
                    HtmlBody = $@"
                    <h3>Estimado {client.FirstName} {client.LastName},</h3>
                    <p>Se ha realizado un pago a su tarjeta de crédito:</p>
                    <ul>
                    <li><strong>Monto pagado:</strong> {actualPayment:C}</li>
                    <li><strong>Cuenta origen:</strong> ****{lastFourAccount}</li>
                    <li><strong>Tarjeta:</strong> ****{lastFourCard}</li>
                    <li><strong>Fecha:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}</li>
                    </ul>
                    <p>Gracias por confiar en Artemis Banking.</p>"
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<ResultDto<TransactionConfirmDto>> ValidateLoanPaymentAsync(
          string accountNumber, string loanNumber, decimal amount)
        {
            var result = new ResultDto<TransactionConfirmDto>();

            try
            {
                var account = await _savingsAccountRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);

                if (account == null || !account.IsActive)
                {
                    result.IsError = true;
                    result.Message = "El número de cuenta origen no es válido o está inactiva";
                    return result;
                }

                if (account.Balance < amount)
                {
                    result.IsError = true;
                    result.Message = "Fondos insuficientes en la cuenta origen";
                    return result;
                }

                var loan = await _loanRepository.GetAllQueryAsync()
                    .Include(l => l.Installments)
                    .FirstOrDefaultAsync(l => l.LoanNumber == loanNumber);

                if (loan == null || !loan.IsActive)
                {
                    result.IsError = true;
                    result.Message = "El número de préstamo ingresado no es válido o está completado";
                    return result;
                }

                var loanOwner = await _accountServiceForApp.GetUserById(loan.UserId);
                var outstandingAmount = loan.Installments?
                    .Where(i => !i.IsPaid)
                    .Sum(i => i.PaymentAmount) ?? 0;

                result.IsError = false;
                result.Result = new TransactionConfirmDto
                {
                    AccountId = account.Id,
                    AccountNumber = accountNumber,
                    HolderName = $"{loanOwner!.FirstName} {loanOwner.LastName}",
                    Amount = amount,
                    OutstandingAmount = outstandingAmount
                };
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<bool> ConfirmLoanPaymentAsync(
            string accountNumber, string loanNumber, decimal amount, string cashierId)
        {
            try
            {
                var account = await _savingsAccountRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);

                var loan = await _loanRepository.GetAllQueryAsync()
                    .Include(l => l.Installments).FirstOrDefaultAsync(l => l.LoanNumber == loanNumber);

                if (account == null || loan == null) return false;

                var remainingAmount = amount;
                var pendingInstallments = loan.Installments!.Where(i => !i.IsPaid).OrderBy(i => i.PaymentDate).ToList();

                int paidCount = 0;

                foreach (var installment in pendingInstallments)
                {
                    if (remainingAmount <= 0) break;

                    if (remainingAmount >= installment.PaymentAmount)
                    {
                        remainingAmount -= installment.PaymentAmount;
                        installment.IsPaid = true;
                        installment.IsLate = false;

                       paidCount++;
                    }
                    else
                    {
                        break;
                    }
                }

                if (paidCount == 0)
                {
                    return false;
                }

                var actualPayment = amount - remainingAmount;

                account.Balance -= actualPayment;
                await _savingsAccountRepository.UpdateAsync(account.Id, account);

                if (remainingAmount > 0)
                {
                    account.Balance += remainingAmount;
                }

                await _loanRepository.UpdateAsync(loan.Id, loan);

                var transaction = new Transaction
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    Amount = actualPayment,
                    Type = (int)TypeCard.Debit,
                    TypeTransaction = (int)TypeTransaction.LoanPaid,
                    Status = (int)StatusTransaction.Approved,
                    SavingsAccountId = account.Id,
                    CashierId = cashierId,
                    Source = account.AccountNumber,
                    Beneficiary = loan.LoanNumber
                };

                await _transactionRepository.AddAsync(transaction);

                var client = await _accountServiceForApp.GetUserById(loan.UserId);
                var lastFour = account.AccountNumber[^4..];

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = client!.Email,
                    Subject = $"Pago realizado al préstamo {loanNumber}",
                    HtmlBody = $@"
                    <h3>Estimado {client.FirstName} {client.LastName},</h3>
                    <p>Se ha realizado un pago a su préstamo:</p>
                    <ul>
                    <li><strong>Préstamo:</strong> {loanNumber}</li>
                    <li><strong>Monto pagado:</strong> {actualPayment:C}</li>
                    <li><strong>Cuenta origen:</strong> ****{lastFour}</li>
                    <li><strong>Fecha:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}</li>
                    </ul>
                    <p>Gracias por confiar en Artemis Banking.</p>"
                });

                return true;
            }
            catch
            {
                return false;
            }


        }

        public async Task<ResultDto<TransactionConfirmDto>> ValidateThirdPartyTransactionAsync(string accountOrigin, 
            string accountDestination, decimal amount)
        {
            var result = new ResultDto<TransactionConfirmDto>();

            try
            {
                var origin = await _savingsAccountRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(x => x.AccountNumber == accountOrigin);

                if (origin == null || !origin.IsActive)
                {
                    result.IsError = true;
                    result.Message = "El número de cuenta origen no es válido o está inactiva";
                    return result;
                }

                if (origin.Balance < amount)
                {
                    result.IsError = true;
                    result.Message = "Fondos insuficientes en la cuenta origen";
                    return result;
                }

                var destination = await _savingsAccountRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(x => x.AccountNumber == accountDestination);

                if (destination == null || !destination.IsActive)
                {
                    result.IsError = true;
                    result.Message = "El número de cuenta destino no es válido o está inactiva";
                    return result;
                }

                if (accountOrigin == accountDestination)
                {
                    result.IsError = true;
                    result.Message = "La cuenta origen y destino no pueden ser la misma";
                    return result;
                }

                var destOwner = await _accountServiceForApp.GetUserById(destination.UserId);

                result.IsError = false;
                result.Result = new TransactionConfirmDto
                {
                    AccountId = destination.Id,
                    AccountNumber = accountDestination,
                    HolderName = $"{destOwner!.FirstName} {destOwner.LastName}",
                    Amount = amount
                };
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<bool> ConfirmThirdPartyTransactionAsync(string accountOrigin, string accountDestination, 
            decimal amount, string cashierId)
        {
            try
            {
                var origin = await _savingsAccountRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountOrigin);

                var destination = await _savingsAccountRepository.GetAllQueryAsync()
                    .FirstOrDefaultAsync(a => a.AccountNumber == accountDestination);

                if (origin == null || destination == null) return false;

                origin.Balance -= amount;
                await _savingsAccountRepository.UpdateAsync(origin.Id, origin);

                destination.Balance += amount;
                await _savingsAccountRepository.UpdateAsync(destination.Id, destination);

                var debitTransaction = new Transaction
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    Amount = amount,
                    Type = (int)TypeCard.Debit,
                    TypeTransaction = (int)TypeTransaction.Transfer,
                    Status = (int)StatusTransaction.Approved,
                    SavingsAccountId = origin.Id,
                    CashierId = cashierId,
                    Source = origin.AccountNumber,
                    Beneficiary = destination.AccountNumber
                };

                await _transactionRepository.AddAsync(debitTransaction);

                var creditTransaction = new Transaction
                {
                    Id = 0,
                    Date = DateTime.UtcNow,
                    Amount = amount,
                    Type = (int)TypeCard.Credit,
                    TypeTransaction = (int)TypeTransaction.Transfer,
                    Status = (int)StatusTransaction.Approved,
                    SavingsAccountId = destination.Id,
                    CashierId = cashierId,
                    Source = origin.AccountNumber,
                    Beneficiary = destination.AccountNumber
                };

                await _transactionRepository.AddAsync(creditTransaction);

                var originOwner = await _accountServiceForApp.GetUserById(origin.UserId);
                var destOwner = await _accountServiceForApp.GetUserById(destination.UserId);

                var lastFourOrigin = origin.AccountNumber[^4..];
                var lastFourDest = destination.AccountNumber[^4..];

             
                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = originOwner!.Email,
                    Subject = $"Transacción realizada a la cuenta ****{lastFourDest}",
                    HtmlBody = $@"
                    <h3>Estimado {originOwner.FirstName} {originOwner.LastName},</h3>
                    <p>Se ha realizado una transacción desde su cuenta:</p>
                    <ul>
                    <li><strong>Monto transferido:</strong> {amount:C}</li>
                    <li><strong>Cuenta destino:</strong> ****{lastFourDest}</li>
                    <li><strong>Fecha:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}</li>
                    </ul>
                    <p>Gracias por confiar en Artemis Banking.</p>"
                });

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = destOwner!.Email,
                    Subject = $"Transacción recibida desde la cuenta ****{lastFourOrigin}",
                    HtmlBody = $@"
                    <h3>Estimado {destOwner.FirstName} {destOwner.LastName},</h3>
                    <p>Se ha recibido una transacción en su cuenta:</p>
                    <ul>
                    <li><strong>Monto recibido:</strong> {amount:C}</li>
                    <li><strong>Cuenta origen:</strong> ****{lastFourOrigin}</li>
                    <li><strong>Fecha:</strong> {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss}</li>
                    </ul>
                    <p>Gracias por confiar en Artemis Banking.</p>"
                });

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
