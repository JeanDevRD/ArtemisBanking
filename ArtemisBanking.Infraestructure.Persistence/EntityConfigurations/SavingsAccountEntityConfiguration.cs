using ArtemisBanking.Core.Domain.Common.Enum;
using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBanking.Infraestructure.Persistence.EntityConfigurations
{
    public class SavingsAccountEntityConfiguration : IEntityTypeConfiguration<SavingsAccount>
    {
        public void Configure(EntityTypeBuilder<SavingsAccount> builder)
        {
            #region Basic Configuration
            builder.HasKey(b => b.Id);
            builder.ToTable("SavingsAccounts");
            #endregion

            #region Property
            builder.Property(sa => sa.AccountNumber).IsRequired().HasMaxLength(40);
            builder.Property(sa => sa.Balance).IsRequired().HasColumnType("decimal(18,2)").HasDefaultValue(0.00);
            builder.Property(sa => sa.Type).IsRequired().HasDefaultValue((int)TypeSavingAccount.None);
            builder.Property(sa => sa.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(sa => sa.CreatedAt).IsRequired();
            builder.Property(sa => sa.UserId).IsRequired().HasMaxLength(100);
            builder.Property(sa => sa.AssignedByUserId).IsRequired(false).HasMaxLength(100);
            #endregion

            #region Relations
            builder.HasMany(cc => cc.Transactions)
                .WithOne(cd => cd.SavingsAccount)
                .HasForeignKey(cd => cd.SavingsAccountId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
