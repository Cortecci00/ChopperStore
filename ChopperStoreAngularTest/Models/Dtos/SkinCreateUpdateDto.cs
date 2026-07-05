using System.ComponentModel.DataAnnotations;

namespace ChopperStoreAngularTest.Models.Dtos
{
    public class SkinCreateUpdateDto
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string name { get; set; }

        public float skinFloat { get; set; }

        public int pattern { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string rarity { get; set; }

        public double price { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int categoryId { get; set; }

        public string? photoUrl { get; set; }
        public string? inspectLink { get; set; }
    }
}
