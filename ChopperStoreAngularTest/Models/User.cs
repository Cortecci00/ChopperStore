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

        public string name { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool isAdmin { get; set; }
        public bool isBlocked { get; set; }
    }
}
