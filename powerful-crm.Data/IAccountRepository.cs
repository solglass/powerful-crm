using powerful_crm.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace powerful_crm.Data
{
    public interface IAccountRepository
    {
        int AddAccount(AccountDto dto);
        int DeleteAccount(int id);
        AccountDto GetAccountById(int id);
        Task<List<AccountDto>> GetAccountsByLeadIdAsync(int leadId);
    }
}