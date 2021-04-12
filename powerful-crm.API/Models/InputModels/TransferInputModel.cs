using powerful_crm.API.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Models.InputModels
{
    public class TransferInputModel
    {
        [Range(1, int.MaxValue)]
        public int SenderId { get; set; }
        [Range(1, int.MaxValue)]
        public int RecipientId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        [CustomCurrencyValidation]
        public string CurrentCurrency { get; set; }
        [Required]
        [CustomCurrencyValidation]
        public string AccountCurrency { get; set; }
    }
}
