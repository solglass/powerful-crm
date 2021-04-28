using powerful_crm.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.Models
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Currency Currency { get; set; }
        public LeadDto LeadDto { get; set; }
    }
}
