using powerful_crm.API.Attributes;
using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Models.InputModels
{
    public class TransactionInputModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int AccountId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        [CustomCurrencyValidation]
        public string Currency { get; set; }
    }
}
