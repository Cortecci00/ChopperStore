using System.ComponentModel.DataAnnotations;

namespace ChopperStoreAngularTest.Models.Dtos
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "El token es obligatorio")]
        public string Token { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [MinLength(5, ErrorMessage = "La contraseña debe tener al menos {1} letras")]
        public string NewPassword { get; set; }
    }
}
