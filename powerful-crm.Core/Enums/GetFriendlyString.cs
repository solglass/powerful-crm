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
                SearchType.Beginning => name + '%',
                SearchType.Сontent => '%' + name + '%',
                SearchType.Ending=>'%'+name,
                _ => name
            };
            return friendlyName;

        }
    }
}
