using Microsoft.AspNetCore.Http;
using powerful_crm.Core.Enums;
using System;
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

        public bool CheckCurrencyIsDefined(string currency)
        {
            return Enum.IsDefined(typeof(Currency), currency);
        }
    }
}
