using powerful_crm.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace powerful_crm.API.Attributes
{
    public class NullableCustomDateTimeValidation : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            return !(string.IsNullOrWhiteSpace((string)value)) ? DateTime.TryParseExact(
                (string)value,
                Constants.DATE_FORMAT,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime result) : true;
        }
    }
}
