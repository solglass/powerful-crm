using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Net;


namespace powerful_crm.Core.CustomExceptions
{
    public class CustomValidationException : CustomException
    {
        public CustomValidationException(ModelStateDictionary modelState)
        {
            StatusCode = (int)HttpStatusCode.Conflict;
            ErrorMessage = ""; 
            foreach (var state in modelState)
            {
                ErrorMessage += $"Invalid format {state.Key}{System.Environment.NewLine}";
            }
        }
    }
}
