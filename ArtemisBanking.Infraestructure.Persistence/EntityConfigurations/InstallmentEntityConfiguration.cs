using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBanking.Infraestructure.Persistence.EntityConfigurations
{
    public class InstallmentEntityConfiguration : IEntityTypeConfiguration<Installment>
    {
        public void Configure(EntityTypeBuilder<Installment> builder)
        {
            #region Basic Configuration
            builder.HasKey(b => b.Id);
            builder.ToTable("Installments");
            #endregion

            #region Property
            builder.Property(i => i.PaymentDate).IsRequired();
            builder.Property(i => i.PaymentAmount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0.00);
            builder.Property(i => i.IsPaid).IsRequired().HasDefaultValue(false);
            builder.Property(i => i.IsLate).IsRequired().HasDefaultValue(false);
            builder.Property(i => i.LoanId).IsRequired();
            #endregion
        }
    }
}
