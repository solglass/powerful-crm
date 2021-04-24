using powerful_crm.Core.Enums;
using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.OutputModels
{
    public class AccountOutputModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Currency { get; set; }
        public LeadDto LeadDto { get; set; }
    }
}
