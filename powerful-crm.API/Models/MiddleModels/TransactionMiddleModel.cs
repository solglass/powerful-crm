using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.MiddleModels
{
    public class TransactionMiddleModel
    {
        public int LeadId { get; set; }
        public decimal Amount { get; set; }
        public string CurrencyPair { get; set; }
    }
}
