using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.Enums;
using System;
using System.Linq;
using System.Security.Claims;

namespace powerful_crm.Business
{
    public class CheckerService : ICheckerService
    {
        public bool CheckIfUserIsAllowed(int leadId, HttpContext httpContext)
        {
            return leadId.ToString() == httpContext.User.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value
                || httpContext.User.IsInRole(Role.Administrator.ToString());
        }

        public bool CheckCurrencyIsDefined(string currency)
        {
            return Enum.IsDefined(typeof(Currency), currency);
        }
        public bool CheckIfUserIsAllowed(HttpContext httpContext)
        {
            return httpContext.User.IsInRole(Role.Administrator.ToString());
        }

        public void CheckInputModel(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
                throw new CustomValidationException(modelState);
        }

    }
}
