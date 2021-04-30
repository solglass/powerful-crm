namespace powerful_crm.Business
{
    public interface ISecurityService
    {
        string GetHash(string password);
        bool VerifyPassword(string hashFromDb, string enteredPassword);
    }
}
