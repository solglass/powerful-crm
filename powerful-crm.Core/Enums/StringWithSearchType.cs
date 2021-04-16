using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.Enums
{
    public static class StringWithSearchType
    {
        public static string GetStringWithSearchType(string name, SearchType searchType)
        {

            string stringWithSearchType = searchType switch
            {
                SearchType.Beginning => name + '%',
                SearchType.Сontent => '%' + name + '%',
                SearchType.Ending=>'%'+name,
                SearchType.Equal=>name,
                _ => name
            };
            return stringWithSearchType;

        }
    }
}
