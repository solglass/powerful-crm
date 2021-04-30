using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Models.InputModels
{
    public class LoginInputModel
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
