using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Core.Configs
{
    public class AuthOptions
    {
        public const string ISSUER = "powerful-crm.Api";
        public const string AUDIENCE = "PowerfulCRM";
        const string KEY = "sdcbsdcbhopwqeihf@#$@@#$45dfolmjioioqhcx";
        public const int LIFETIME = 2880;
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
