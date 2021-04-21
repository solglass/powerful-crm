using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business.Models
{
    public class AuthenticateResponse
    {
        public string Token { get; set; }
        public string LeadLogin { get; set; }
    }
}
