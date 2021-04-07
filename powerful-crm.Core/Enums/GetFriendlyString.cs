using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.Enums
{
    public static class GetFriendlyString
    {
        public static string GetFriendlyType(string name, SearchType searchType)
        {

            string friendlyName = searchType switch
            {
                SearchType.Сontent => '%' + name + '%',
                SearchType.Beginning => name + '%',
                _ => name
            };
            return friendlyName;

        }
    }
}
