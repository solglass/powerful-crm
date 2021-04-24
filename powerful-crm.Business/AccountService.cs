using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business
{
    public class AccountService : IAccountService
    {
        public int AddAccount(AccountDto dto)
        {
            return AddAccount(dto);
        }
        public int DeleteAccount(int id)
        {
            return DeleteAccount(id);
        }
        public AccountDto GetAccountById(int id)
        {
            return GetAccountById(id);
        }
    }
}
