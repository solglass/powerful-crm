using powerful_crm.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business.Models
{
    public class AuthenticationResponse
    {
        public string Token { get; set; }
        public string LeadLogin { get; set; }
        public Role Role { get; set; }
    }
}
