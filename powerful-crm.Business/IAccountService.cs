using powerful_crm.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
    public interface IAccountService
    {
        int AddAccount(AccountDto dto);
        int DeleteAccount(int id);
        AccountDto GetAccountById(int id);
        Task<List<AccountDto>> GetAccountsByLeadIdAsync(int leadId);
    }
}