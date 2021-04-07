using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.InputModels
{
    public class BalanceInputModel
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }
}
