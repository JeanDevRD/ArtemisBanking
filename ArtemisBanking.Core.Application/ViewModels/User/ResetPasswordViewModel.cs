
using System.ComponentModel.DataAnnotations;

namespace ArtemisBanking.Core.Application.ViewModels.User
{
    public class ResetPasswordRequestViewModel
    {
        [Required]
        [DataType(DataType.Text)]
        public required string UserId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public required string Token { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no son iguales")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
