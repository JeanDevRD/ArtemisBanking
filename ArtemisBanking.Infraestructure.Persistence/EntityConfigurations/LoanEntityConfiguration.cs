using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBanking.Infraestructure.Persistence.EntityConfigurations
{
    public class LoanEntityConfiguration : IEntityTypeConfiguration<Loan>
    {
        public void Configure(EntityTypeBuilder<Loan> builder)
        {
            #region Basic Configuration
            builder.HasKey(b => b.Id);
            builder.ToTable("Loans");
            #endregion

            #region Property
            builder.Property(l => l.LoanNumber).IsRequired().HasMaxLength(20);
            builder.Property(l => l.Amount).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0.00);
            builder.Property(l => l.TermMonths).IsRequired().HasDefaultValue(0);
            builder.Property(l => l.AnnualInterestRate).IsRequired().HasColumnType("decimal(5,2)").HasDefaultValue(0.00);
            builder.Property(l => l.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(l => l.ApprovedAt).IsRequired();
            builder.Property(l => l.ApprovedByUserId).IsRequired().HasMaxLength(100);
            builder.Property(l => l.UserId).IsRequired().HasMaxLength(100);
            #endregion

            #region Relations
            builder.HasMany(cc => cc.Installments)
                .WithOne(cd => cd.Loan)
                .HasForeignKey(cd => cd.LoanId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
