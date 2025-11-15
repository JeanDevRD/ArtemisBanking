
using ArtemisBanking.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtemisBanking.Infraestructure.Persistence.EntityConfigurations
{
    public class BeneficiaryEntityConfiguration : IEntityTypeConfiguration<Beneficiary>
    {
        public void Configure(EntityTypeBuilder<Beneficiary> builder)
        {
            builder.Property(b => b.FirstName).HasMaxLength(50).IsRequired();
            builder.Property(b => b.LastName).HasMaxLength(50).IsRequired();
            builder.Property(b => b.AccountNumber).HasMaxLength(9).IsRequired();
            builder.Property(b => b.UserId).IsRequired();

            // NUEVOS CAMPOS
            builder.Property(b => b.Bank).HasMaxLength(100).IsRequired();
            builder.Property(b => b.AccountType).IsRequired();
        }
    }
}
