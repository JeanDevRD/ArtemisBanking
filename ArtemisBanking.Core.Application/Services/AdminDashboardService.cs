using ArtemisBanking.Core.Application.Dtos.AdminDashboard;
using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services
{
    public class AdminDashboardService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IAccountServiceForApp _clientForApp;
        private readonly IAccountServiceForApi _clientForApi;
        private readonly ILoanRepository _loanRepo;
        private readonly ICreditCardRepository _creditCardRepo;
        private readonly ISavingsAccountRepository _savingsAccountRepo;
        private readonly ICardTransactionRepository _cardTransactionRepo;
        public AdminDashboardService(
            ITransactionRepository transactionRepo,
            ILoanRepository loanRepo,
            ICreditCardRepository creditCardRepo,
            ISavingsAccountRepository savingsAccountRepo,
            ICardTransactionRepository cardTransactionRepo,
            IAccountServiceForApp clientForApp,
            IAccountServiceForApi clientForApi)
        {
            _transactionRepo = transactionRepo;
            _loanRepo = loanRepo;
            _creditCardRepo = creditCardRepo;
            _savingsAccountRepo = savingsAccountRepo;
            _cardTransactionRepo = cardTransactionRepo;
            _clientForApp = clientForApp;
            _clientForApi = clientForApi;
        }


        public async Task<ResultDto<HistoricTransactionDto>> TotalTransactions() 
        {
            var result = new ResultDto<HistoricTransactionDto>();
            try 
            { 
                var totalTransactions = await _transactionRepo.GetAllQueryAsync().CountAsync();
                if(totalTransactions == 0)
                {
                    result.IsError = true;
                    result.Message = "Aun no hya historial de transacciones.";
                    return result;
                }

                result.Result = new HistoricTransactionDto 
                { 
                    TotalHistoricTransactions = totalTransactions
                };

            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<TotalClientsDto>> TotalClientsForWebApp() 
        {
            var result = new ResultDto<TotalClientsDto>();
            try
            {
                var client = await _clientForApp.GetAllUser();
                if(client == null) 
                {
                    result.IsError = true;
                    result.Message = "No hay clientes inactivos o activos";
                    return result;
                }


                var active = client.Where(x => x.IsActive == true).Count();
                if(active == 0) 
                {
                    result.IsError = true;
                    result.Message = "No hay clientes activos";
                    return result;
                }

                var inactive = client.Where(x => x.IsActive == false).Count();
                if (inactive == 0)
                {
                    result.IsError = true;
                    result.Message = "No hay clientes inactivos";
                    return result;
                }


                var clients = new TotalClientsDto 
                {
                    TotalClientsActive = active, 
                    TotalClientsInactive = inactive
                };


            }
            catch (Exception ex) 
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<TotalClientsDto>> TotalClientsForWebApi()
        {
            var result = new ResultDto<TotalClientsDto>();
            try
            {
                var client = await _clientForApi.GetAllUser();
                if (client == null)
                {
                    result.IsError = true;
                    result.Message = "No hay clientes inactivos o activos";
                    return result;
                }


                var active = client.Where(x => x.IsActive == true).Count();
                if (active == 0)
                {
                    result.IsError = true;
                    result.Message = "No hay clientes activos";
                    return result;
                }

                var inactive = client.Where(x => x.IsActive == false).Count();
                if (inactive == 0)
                {
                    result.IsError = true;
                    result.Message = "No hay clientes inactivos";
                    return result;
                }


                var clients = new TotalClientsDto
                {
                    TotalClientsActive = active,
                    TotalClientsInactive = inactive
                };


            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<LoadCountDto>> TotalLoads() 
        {
            var result = new ResultDto<LoadCountDto>();
            try
            {
                var Loans = await _loanRepo.GetAllQueryAsync().Where(x => x.IsActive == true).CountAsync();
                if (Loans == 0) 
                {
                    result.IsError = true;
                    result.Message = "No hay prestamos activos en este momento";
                    return result;
                }

                var totalLoans = new LoadCountDto { TotalLoad = Loans};
                result.IsError = false;
                result.Result = totalLoans;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<CreditCardCountDto>> TotalCreditCardActive()
        {
            var result = new ResultDto<CreditCardCountDto>();
            try
            {
                var creditCard = await _creditCardRepo.GetAllQueryAsync().Where(x => x.IsActive == true).CountAsync();
                if (creditCard == 0) 
                {
                    result.IsError = true;
                    result.Message = "No hay emitidas que esten activas";
                    return result;
                }

                var totalCreditCard = new CreditCardCountDto 
                { 
                    TotalActiveCreditCard = creditCard 
                };

                result.IsError = false;
                result.Result = totalCreditCard;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<TotalSavingAccountDto>> TotalSavingAccount()
        {
            var result = new ResultDto<TotalSavingAccountDto>();
            try
            {
                var savingAccount = await _savingsAccountRepo.GetAllQueryAsync().Where(x => x.IsActive == true).CountAsync();
                if (savingAccount == 0) 
                {
                    result.IsError = true;
                    result.Message = "No hay cuentas de ahorro abiertas";
                    return result;
                }

                var totalSavingAccount = new TotalSavingAccountDto 
                { 
                    TotalSavingAccount = savingAccount 
                };

                result.IsError = false;
                result.Result = totalSavingAccount;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }

            return result;
        }

        public async Task<ResultDto<PaymentHistoryDto>> TotalPaymentHistory()
        {
            var result = new ResultDto<PaymentHistoryDto>();
            try
            {
                var totalPaymentHistory = await _transactionRepo.GetAllQueryAsync()
                    .Where(x => x.TypeTransaction == (int)TypeTransaction.Paid).CountAsync();

                if (totalPaymentHistory == 0)
                {
                    result.IsError = true;
                    result.Message = "Aun no hya historial de pagos.";
                    return result;
                }
                var paymentHistoryForDay = await _transactionRepo.GetAllQueryAsync()
                .Where(x => x.Date.Date == DateTime.UtcNow.Date &&  x.TypeTransaction == (int)TypeTransaction.Paid).CountAsync();

                var paymentHistoryDto = new PaymentHistoryDto
                {
                    TotalPaymentHistory = totalPaymentHistory,
                    PaymentHistoryForDay = paymentHistoryForDay
                };
                result.IsError = false;
                result.Result = paymentHistoryDto;
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<ResultDto<TotalFinancialProductDto>> TotalFinancialProducts() 
        { 
         var result = new ResultDto<TotalFinancialProductDto>();
            try 
            {
                int savingsAccounts = await _savingsAccountRepo.GetAllQueryAsync().CountAsync();
                int creditCards = await _creditCardRepo.GetAllQueryAsync().CountAsync();
                int Loans = await _loanRepo.GetAllQueryAsync().CountAsync();

                int totalProductsCount = savingsAccounts + creditCards + Loans;

                if(totalProductsCount == 0) 
                {
                    result.IsError = true;
                    result.Message = "Aun no hay productos financieros asignados a clientes";
                    return result;
                }
                var totalProducts = new TotalFinancialProductDto
                {
                    TotalFinancialProductsCount = totalProductsCount,
                };
             
                result.IsError = false;
                result.Result = totalProducts;
            } 
            catch(Exception ex) 
            { 
                result.IsError = true;
                result.Message = ex.Message;

            }
            return result;
        }

        public async Task<ResultDto<LoanAverageClientDto>> AverageLoansPerClient()
        {
            var result = new ResultDto<LoanAverageClientDto>();
            try
            {
                var clients = await _clientForApi.GetAllUser(isActive: true);
                int totalClients = clients.Count;

                if (totalClients == 0)
                {
                    result.IsError = true;
                    result.Message = "No hay clientes activos registrados.";
                    return result;
                }

                decimal totalDebt = 0;

                var activeLoans = await _loanRepo.GetAllListIncluideAsync(new List<string> { "Installments" });
                var activeLoansFiltered = activeLoans.Where(l => l.IsActive).ToList();

                foreach (var loan in activeLoansFiltered)
                {
                    if (loan.Installments != null)
                    {
                        totalDebt += loan.Installments.Where(i => !i.IsPaid).Sum(i => i.PaymentAmount);
                    }
                }

               
                var activeCreditCards = await _creditCardRepo.GetAllListAsync();

                totalDebt += activeCreditCards.Where(c => c.IsActive).Sum(c => c.CurrentDebt);

                decimal averageDebt = totalClients > 0 ? totalDebt / totalClients : 0;

                result.IsError = false;
                result.Result = new LoanAverageClientDto
                {
                    AverageLoanAmount = (double)averageDebt,
                    TotalClients = totalClients,
                };
            }
            catch (Exception ex)
            {
                result.IsError = true;
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
