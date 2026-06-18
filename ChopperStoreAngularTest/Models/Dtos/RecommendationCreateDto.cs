using System.ComponentModel.DataAnnotations;

namespace ChopperStoreAngularTest.Models.Dtos
{
    public class RecommendationCreateDto
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [MaxLength(900, ErrorMessage = "El campo {0} no puede superar los {1} caracteres")]
        public string Text { get; set; }
    }
}
