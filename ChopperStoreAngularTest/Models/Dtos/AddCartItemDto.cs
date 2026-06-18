using System.ComponentModel.DataAnnotations;

namespace ChopperStoreAngularTest.Models.Dtos
{
    public class AddCartItemDto
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int SkinId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1")]
        public int Quantity { get; set; }
    }
}
