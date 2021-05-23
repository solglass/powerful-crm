using powerful_crm.Business.Models;
using powerful_crm.Core.Models;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
    public interface IAuthenticationService
    {
        Task<LeadDto> GetAuthenticatedLeadAsync(string login);
        AuthenticationResponse GenerateToken(LeadDto lead);
    }
}
