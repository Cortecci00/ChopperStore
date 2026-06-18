using System.ComponentModel.DataAnnotations;

namespace ChopperStoreAngularTest.Models.Dtos
{
    public class ContactMessageCreateDto
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string name { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string lastname { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string email { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(900, ErrorMessage = "El campo {0} no puede superar los {1} caracteres")]
        public string message { get; set; }
    }
}
