using powerful_crm.Core;
using System;

namespace powerful_crm.Business
{
    class SecurityService:ISecurityService
    {
        private static string GetRandomSalt()
        {
            return BCrypt.Net.BCrypt.GenerateSalt(12);
        }
        public string GetHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, GetRandomSalt());
        }
        public bool VerifyPassword (string hashFromDb, string enteredPassword)
        {
            return BCrypt.Net.BCrypt.Verify(enteredPassword, hashFromDb);
        }
    }
}
