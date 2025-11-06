using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBanking.Infraestructure.Persistence.EntityConfigurations
{
    public class CreditCardEntityConfiguration : IEntityTypeConfiguration<CreditCard>
    {
        public void Configure(EntityTypeBuilder<CreditCard> builder)
        {
            #region Basic Configuration
            builder.HasKey(b => b.Id);
            builder.ToTable("CreditCards");
            #endregion

            #region Property
            builder.Property(cc => cc.CardNumber).HasMaxLength(100).IsRequired();
            builder.Property(cc => cc.CvcHash).HasMaxLength(int.MaxValue).IsRequired();
            builder.Property(cc => cc.CreditLimit).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(cc => cc.CurrentDebt).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(cc => cc.ExpirationDate).IsRequired();
            builder.Property(cc => cc.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(cc => cc.UserId).HasMaxLength(100).IsRequired();
            builder.Property(cc => cc.AssignedByUserId).HasMaxLength(100).IsRequired();
            #endregion

            #region Relations
            builder.HasMany(cc => cc.CardTransactions)
                .WithOne(cd => cd.CreditCard)
                .HasForeignKey(cd => cd.CreditCardId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
    }
}
