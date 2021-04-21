using powerful_crm.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace powerful_crm.API.Attributes
{
    public class CustomCurrencyValidation : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            var currency = (string)value;
            return Enum.IsDefined(typeof(Currency), currency);    
        }
    }
}
