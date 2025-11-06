using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ArtemisBanking.Infraestructure.Persistence.Contexts
{
    public class ArtemisBankingContextSqlServer : DbContext 
    {
        public ArtemisBankingContextSqlServer(DbContextOptions<ArtemisBankingContextSqlServer> options)
            : base(options) {}

        public DbSet<Beneficiary> Beneficiaries { get; set; }
        public DbSet<CardTransaction> CardTransactions { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }
        public DbSet<Installment> Installments { get; set; }
        public DbSet<Loan> Loans { get; set; }  
        public DbSet<Merchant> Marchants { get; set; }
        public DbSet<SavingsAccount> SavingsAccounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
