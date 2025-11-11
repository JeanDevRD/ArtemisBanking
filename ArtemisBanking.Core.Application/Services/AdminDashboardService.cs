using ArtemisBanking.Core.Application.Dtos.AdminDashboard;
using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ArtemisBanking.Core.Application.Services
{
    public class AdminDashboardService
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly ILoanRepository _loanRepo;
        private readonly ICreditCardRepository _creditCardRepo;
        private readonly ISavingsAccountRepository _savingsAccountRepo;
        private readonly ICardTransactionRepository _cardTransactionRepo;
        public AdminDashboardService(
            ITransactionRepository transactionRepo,
            ILoanRepository loanRepo,
            ICreditCardRepository creditCardRepo,
            ISavingsAccountRepository savingsAccountRepo,
            ICardTransactionRepository cardTransactionRepo)
        {
            _transactionRepo = transactionRepo;
            _loanRepo = loanRepo;
            _creditCardRepo = creditCardRepo;
            _savingsAccountRepo = savingsAccountRepo;
            _cardTransactionRepo = cardTransactionRepo;
        }


        //Total de todas las transacciones OJO AGRAGR LOS METODOS DE TRANSACCIONES AL REPO
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

        //Total de pagos por dia OJO AGREGAR METODOS DE PAGO A PRETAMOS Y TARJETA DE CREDITO


        //Total de todos los pagos OJO AGREGAR METODOS DE PAGO A PRETAMOS Y TARJETA DE CREDITO
    }
}
