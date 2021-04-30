using powerful_crm.API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Models.InputModels
{
    public class AccountInputModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        [CustomCurrencyAccountValidation]
        public int Currency { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int LeadId { get; set; }
    }
}
