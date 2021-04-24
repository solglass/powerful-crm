using powerful_crm.Core.Models;

namespace powerful_crm.Data
{
    public interface IAccountRepository
    {
        int AddAccount(AccountDto dto);
        int DeleteAccount(int id);
        AccountDto GetAccountById(int id);
    }
}