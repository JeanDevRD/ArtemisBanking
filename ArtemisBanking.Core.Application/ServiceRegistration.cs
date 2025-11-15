using ArtemisBanking.Core.Application.Interfaces;
using ArtemisBanking.Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ArtemisBanking.Core.Application
{
    public static class ServiceRegistration
    {
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            #region Configuration
            services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
            #endregion
            #region Services
            services.AddScoped<IBeneficiaryService, BeneficiaryService>();
            services.AddScoped<ISavingsAccountService, SavingsAccountService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ILoanService, LoanService>();
            services.AddScoped<ICreditCardService, CreditCardService>();
            services.AddScoped<IInstallmentService, InstallmentService>();
            services.AddScoped<ICardTransactionService, CardTransactionService>();
            services.AddScoped<IMerchantService, MerchantService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAdminDashboardService, AdminDashboardService>();
           
            #endregion
        }
    }
}
