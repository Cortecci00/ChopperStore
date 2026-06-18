using System.ComponentModel.DataAnnotations;

namespace ChopperStoreAngularTest.Models.Dtos
{
    public class UpdatePhotoDto
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string PhotoUrl { get; set; }
    }
}
