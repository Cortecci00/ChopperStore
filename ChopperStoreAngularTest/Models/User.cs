using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ChopperStoreAngularTest.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? GoogleId { get; set; }
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? username { get; set; }
        [JsonIgnore]
        public string? password { get; set; }
        public bool isAdmin { get; set; }
        public bool isBlocked { get; set; }
        public string? PhotoUrl { get; set; }

    }

    public class CreateUpdate
    {
        [Required(ErrorMessage = "El campo {0} no es correcto")]
        [MinLength(5, ErrorMessage = "El campo {0} debe tener al menos {1} letras")]
        public string username { get; set; }

        [Required(ErrorMessage = "El campo {0} no es correcto")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string email { get; set; }

        [Required(ErrorMessage = "El campo {0} no es correcto")]
        [MinLength(5, ErrorMessage = "El campo {0} debe tener al menos {1} letras")]
        public string password { get; set; }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "El campo email no es correcto")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string email { get; set; }

        [Required(ErrorMessage = "El campo contraseña no es correcto")]
        public string password { get; set; }
    }

    public class GoogleLoginDto
    {
        public string Token { get; set; }
    }

    public class LoginDto
    {
        public string email { get; set; }
        public string password { get; set; }
    }
}
