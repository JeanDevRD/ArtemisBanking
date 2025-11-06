using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBanking.Infraestructure.Persistence.EntityConfigurations
{
    public class MerchantEntityConfiguration : IEntityTypeConfiguration<Merchant>
    {
        public void Configure(EntityTypeBuilder<Merchant> builder)
        {
            #region Basic Configuration
            builder.HasKey(b => b.Id);
            builder.ToTable("Merchants");
            #endregion

            #region Property
            builder.Property(m => m.Name).HasMaxLength(60).IsRequired();
            builder.Property(m => m.Email).HasMaxLength(100).IsRequired();
            builder.Property(m => m.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property(m => m.CreatedAt).IsRequired();
            builder.Property(m => m.AssociatedAccount).HasMaxLength(40).IsRequired();
            builder.Property(m => m.UserId).HasMaxLength(100).IsRequired();
            #endregion
        }
    }
}
