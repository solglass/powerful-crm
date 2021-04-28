using powerful_crm.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
