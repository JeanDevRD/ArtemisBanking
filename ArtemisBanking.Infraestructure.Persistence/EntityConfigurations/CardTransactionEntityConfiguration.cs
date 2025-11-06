using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBanking.Infraestructure.Persistence.EntityConfigurations
{
    public class CardTransactionEntityConfiguration : IEntityTypeConfiguration<CardTransaction>
    {
        public void Configure(EntityTypeBuilder<CardTransaction> builder)
        {
            #region Basic Configuration
            builder.HasKey(b => b.Id);
            builder.ToTable("CardTransactions");
            #endregion

            #region Property
            builder.Property(ct => ct.Date).IsRequired();
            builder.Property(ct => ct.Amount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0.00);
            builder.Property(ct => ct.Merchant).IsRequired().HasMaxLength(100);
            builder.Property(ct => ct.Status).IsRequired().HasDefaultValue((int)StatusTransaction.None);
            builder.Property(ct => ct.CreditCardId).IsRequired();
            #endregion
        }
    }
}
