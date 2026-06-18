using System.ComponentModel.DataAnnotations;

namespace ChopperStoreAngularTest.Models.Dtos
{
    public class UpdateSteamTradeUrlDto
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string SteamTradeUrl { get; set; }
    }
}
