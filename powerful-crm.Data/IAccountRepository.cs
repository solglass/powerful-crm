using powerful_crm.Core.Models;
using System.Collections.Generic;

namespace powerful_crm.Data
{
    public interface IAccountRepository
    {
        int AddAccount(AccountDto dto);
        int DeleteAccount(int id);
        AccountDto GetAccountById(int id);
        List<AccountDto> GetAccountsByLeadId(int leadId);
    }
}