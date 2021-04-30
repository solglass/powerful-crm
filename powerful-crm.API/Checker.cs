using Microsoft.AspNetCore.Http;
using powerful_crm.Core.Enums;
using System.Linq;
using System.Security.Claims;

namespace powerful_crm.API
{
    public class Checker
    {
        public bool CheckIfUserIsAllowed(int leadId,HttpContext httpContext)
        {
            return leadId.ToString() == httpContext.User.Claims.Where(t=>t.Type==ClaimTypes.NameIdentifier).FirstOrDefault().Value 
                || httpContext.User.IsInRole(Role.Administrator.ToString());
        }
    }
}
