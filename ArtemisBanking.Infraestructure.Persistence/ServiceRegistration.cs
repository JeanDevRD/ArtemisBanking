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
            var connectionString = confi.GetConnectionString("ConnectionStrings");
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
            #endregion
        }
    }
}
