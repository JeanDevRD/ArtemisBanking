using ArtemisBanking.Core.Application.Dtos.Common;
using ArtemisBanking.Core.Domain.Enum;
using System.ComponentModel.DataAnnotations;

namespace ArtemisBanking.Core.Application.Dtos.Beneficiary
{
    public class BeneficiaryDto : CommonEntityDto<int>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        [Required]
        [RegularExpression(@"^(\d{3}-\d{7}-\d{1}|\d{11})$", ErrorMessage = "Formato de cédula inválido. Use 000-0000000-0 o 11 dígitos.")]
        public required string Cedula { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "El número de cuenta debe tener 9 dígitos.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "El número de cuenta debe contener sólo dígitos.")]
        public required string AccountNumber { get; set; }

        public required string UserId { get; set; }

        // NUEVOS CAMPOS
        public required string Bank { get; set; }
        public required int AccountType { get; set; }
        public string AccountTypeName => ((AccountType)AccountType).ToString();
    }
}
