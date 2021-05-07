using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.OutputModels
{
    public class LeadBalanceOutputModel
    {
        public List<AccountBalanceOutputModel> Accounts { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}
