using ArtemisBanking.Core.Application.Dtos.AdminDashboard;
using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Application.Interfaces;
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


    }
}
