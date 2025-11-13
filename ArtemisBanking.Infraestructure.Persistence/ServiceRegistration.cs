using ArtemisBanking.Core.Domain.Interfaces; // ✅ Agregada
using ArtemisBanking.Infraestructure.Persistence.Contexts;
using ArtemisBanking.Infraestructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArtemisBanking.Infraestructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceLayer(this IServiceCollection services, IConfiguration confi)
        {
            #region Context
            var connectionString = confi.GetConnectionString("DefaultConnection");
            services.AddDbContext<ArtemisBankingContextSqlServer>((serviceProvider, opt) =>
            {
                opt.EnableSensitiveDataLogging();
                opt.UseSqlServer(connectionString, m => m.MigrationsAssembly(typeof(ArtemisBankingContextSqlServer).Assembly.FullName));
            },
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Scoped
            );
            #endregion

            #region Repositories
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IBeneficiaryRepository, BeneficiaryRepository>();
            services.AddScoped<ICardTransactionRepository, CardTransactionRepository>();
            services.AddScoped<ICreditCardRepository, CreditCardRepository>();
            services.AddScoped<IInstallmentRepository, InstallmentRepository>();
            services.AddScoped<ILoanRepository, LoanRepository>();
            services.AddScoped<IMerchantRepository, MerchantRepository>();
            services.AddScoped<ISavingsAccountRepository, SavingsAccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            #endregion
        }
    }
}
