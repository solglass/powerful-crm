using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Models.OutputModels
{
    public class CityInputModel
    {
        [Required]
        [StringLength(100, MinimumLength =2)]
        public string Name { get; set; }
    }
}
