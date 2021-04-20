using powerful_crm.Core;
using System;

namespace powerful_crm.Business
{
    class SecurityService
    {
        public string GetHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password /*+ Constants.SALT*/);
        }
        public bool VerifyPassword (string hashFromDb, string enteredPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword /*+ Constants.SALT*/, hashFromDb);
        }
    }
}
