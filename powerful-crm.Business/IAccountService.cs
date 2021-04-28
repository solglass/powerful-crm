using powerful_crm.Core.Models;
using System.Collections.Generic;

namespace powerful_crm.Business
{
    public interface IAccountService
    {
        int AddAccount(AccountDto dto);
        int DeleteAccount(int id);
        AccountDto GetAccountById(int id);
        List<AccountDto> GetAccountsByLeadId(int leadId);
    }
}