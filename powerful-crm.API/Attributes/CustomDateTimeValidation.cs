﻿using powerful_crm.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;


namespace powerful_crm.API.Attributes
{
    public class CustomDateTimeValidation : ValidationAttribute 
    {

        public override bool IsValid(object value)
        {
            return DateTime.TryParseExact(
                (string)value,
                Constants.DATE_FORMAT,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _);
        }
    }
}
