using System.ComponentModel.DataAnnotations;

namespace ArtemisBanking.Core.Application.ViewModels.User
{
    public class SaveUserViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "El primer nombre es obligatorio")]
        [DataType(DataType.Text)]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "El segundo nombre es obligatorio")]
        [DataType(DataType.Text)]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "El documento de identidad es obligatorio")]
        [DataType(DataType.Text)]
        public required string IdentificationNumber { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio")]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [DataType(DataType.Text)]
        public required string UserName { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required(ErrorMessage = "La confirmación de la contraseña es obligatoria")]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "El rol es obligatoria")]
        [DataType(DataType.Text)]
        public required string Role { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string? Phone { get; set; }

        [DataType(DataType.Currency)]
        public decimal InitialAmount { get; set; } = 0; 
    }
}

