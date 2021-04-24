using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.Enums
{
    public static class FriendlyNames
    {
        public static string GetFriendlyCurrency(Currency currency)
        {

            string friendlyName = currency switch
            {
                Currency.RUB => "RUB",
                Currency.USD => "USD",
                Currency.EUR => "EUR",
                Currency.JPY => "JPY",
                _ => "Некорректная валюта"
            };
            return friendlyName;

        }
    }
}
