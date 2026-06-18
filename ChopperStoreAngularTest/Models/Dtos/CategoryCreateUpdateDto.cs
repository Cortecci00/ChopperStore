using System.ComponentModel.DataAnnotations;

namespace ChopperStoreAngularTest.Models.Dtos
{
    public class CategoryCreateUpdateDto
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string name { get; set; }
    }
}
