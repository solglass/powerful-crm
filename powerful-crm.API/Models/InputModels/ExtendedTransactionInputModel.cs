using powerful_crm.API.Models.InputModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.InputModels
{
    public class ExtendedTransactionInputModel : TransactionInputModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Enter valid email")]
        public string LeadPayPalEmail { get; set; }

        public long IDPaypalBatch { get; set; }
    }
}
