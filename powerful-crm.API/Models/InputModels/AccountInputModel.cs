using powerful_crm.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Models.InputModels
{
    public class AccountInputModel
    {
        public string Name { get; set; }
        public int Currency { get; set; }
        public int LeadId { get; set; }
    }
}
