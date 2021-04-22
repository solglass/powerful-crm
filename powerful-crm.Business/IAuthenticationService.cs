using powerful_crm.Business.Models;
using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business
{
    public interface IAuthenticationService
    {
        LeadDto GetAuthenticatedLead(string login);
        AuthenticationResponse GenerateToken(LeadDto lead);
    }
}
