using System.ComponentModel.DataAnnotations;

namespace ChopperStoreAngularTest.Models.Dtos
{
    public class UpdateProfileDto
    {
        public string? name { get; set; }
        public string? lastname { get; set; }
        public string? username { get; set; }

        [EmailAddress(ErrorMessage = "El formato del email no es válido")]
        public string? email { get; set; }

        public string? phone { get; set; }
    }
}
