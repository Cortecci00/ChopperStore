using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ChopperStoreAngularTest.Models
{
    public class User
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool isAdmin { get; set; }
        public bool isBlocked { get; set; }

    }

    public class CreateUpdate 
    {

        // Actualizar - Crear
        [Required(ErrorMessage = "El campo {0} no es correcto")]
        [MinLength(5, ErrorMessage = "El campo {0} debe tener al menos {1} letras")]
        public string name { get; set; }

        [Required(ErrorMessage = "El campo {0} no es correcto")]
        [MinLength(5, ErrorMessage = "El campo {0} debe tener al menos {1} letras")]
        public string lastname { get; set; }

        [Required(ErrorMessage = "El campo {0} no es correcto")]
        [MinLength(2, ErrorMessage = "El campo {0} debe tener al menos {1} letras")]
        public string email { get; set; }

        [Required(ErrorMessage = "El campo {0} no es correcto")]
        [MinLength(3, ErrorMessage = "El campo {0} debe tener al menos {1} letras")]
        public string phone { get; set; }

        [Required(ErrorMessage = "El campo {0} no es correcto")]
        [MinLength(5, ErrorMessage = "El campo {0} debe tener al menos {1} letras")]
        public string username { get; set; }

        [Required(ErrorMessage = "El campo {0} no es correcto")]
        [MinLength(5, ErrorMessage = "El campo {0} debe tener al menos {1} letras")]
        public string password { get; set; }

        [Required(ErrorMessage = "El campo {0} no es correcto")]
        [MinLength(2, ErrorMessage = "El campo {0} debe tener al menos {1} letras")]
        public bool isAdmin { get; set; }

        public bool isBlocked { get; set; }
    }
}
