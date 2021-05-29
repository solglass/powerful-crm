using powerful_crm.API.Models.InputModels;
using powerful_crm.Business.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.InputModels
{
    public class FullInfoTransactionModel : TransactionInputModel
    {
        //[Required]
        public PayoutInputModel PayoutInputModel { get; set; }

        [Required]
        public string LeadEmail { get; set; }
    }
}
