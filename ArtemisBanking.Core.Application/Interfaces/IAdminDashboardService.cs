using ArtemisBanking.Core.Application.Dtos.AdminDashboard;
using ArtemisBanking.Core.Application.Dtos.Common;

namespace ArtemisBanking.Core.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<ResultDto<LoanAverageClientDto>> AverageLoansPerClient();
        Task<ResultDto<TotalClientsDto>> TotalClientsForWebApi();
        Task<ResultDto<TotalClientsDto>> TotalClientsForWebApp();
        Task<ResultDto<CreditCardCountDto>> TotalCreditCardActive();
        Task<ResultDto<TotalFinancialProductDto>> TotalFinancialProducts();
        Task<ResultDto<LoadCountDto>> TotalLoads();
        Task<ResultDto<PaymentHistoryDto>> TotalPaymentHistory();
        Task<ResultDto<TotalSavingAccountDto>> TotalSavingAccount();
        Task<ResultDto<HistoricTransactionDto>> TotalTransactions();
    }
}