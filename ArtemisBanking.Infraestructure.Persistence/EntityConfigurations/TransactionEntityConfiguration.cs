using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBanking.Infraestructure.Persistence.EntityConfigurations
{
    public class TransactionEntityConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            #region Basic Configuration
            builder.HasKey(b => b.Id);
            builder.ToTable("Transactions");
            #endregion

            #region Property
            builder.Property(t => t.Date).IsRequired();
            builder.Property(t => t.Amount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0.00);
            builder.Property(t => t.Type).IsRequired().HasDefaultValue((int)TypeCard.None);
            builder.Property(t => t.TypeTransaction).IsRequired().HasDefaultValue(0);
            builder.Property(t => t.Source).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Beneficiary).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Status).IsRequired().HasDefaultValue((int)StatusCardTransaction.None);
            builder.Property(t => t.SavingsAccountId).IsRequired();
            builder.Property(t => t.CashierId).IsRequired().HasMaxLength(100);
            #endregion
        }
    }
}
