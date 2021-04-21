using powerful_crm.API.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.InputModels
{
    public class TransactionInputModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int LeadId { get; set; }
        [Required]
        [Range(1, (double)decimal.MaxValue)]
        public decimal Amount { get; set; }
        [Required]
        [CustomCurrencyValidation]
        public string CurrentCurrency { get; set; }
        [Required]
        [CustomCurrencyValidation]
        public string AccountCurrency { get; set; }
    }
}
