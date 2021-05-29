using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace powerful_crm.Business
{
    public interface ICheckerService
    {
        bool CheckCurrencyIsDefined(string currency);
        bool CheckIfUserIsAllowed(HttpContext httpContext);
        bool CheckIfUserIsAllowed(int leadId, HttpContext httpContext);
        void CheckInputModel(ModelStateDictionary modelState);
    }
}