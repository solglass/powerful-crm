using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core
{
    public class TwoFactor
    {
        public static string TwoFactorKey(string email)
        {
            return $"myverysecretkey+{email}";
        }
    }
}
