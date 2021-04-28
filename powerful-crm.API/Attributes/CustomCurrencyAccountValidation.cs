using powerful_crm.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Attributes
{
    public class CustomCurrencyAccountValidation: ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return Enum.IsDefined(typeof(Currency), value);
        }
    }
}
