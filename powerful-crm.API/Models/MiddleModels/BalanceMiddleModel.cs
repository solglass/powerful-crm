using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.MiddleModels
{
    public class BalanceMiddleModel
    {
        public List<int> AccountIds { get; set; }
        public string Currency { get; set; }
    }
}
