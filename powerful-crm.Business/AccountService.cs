using powerful_crm.Core.Models;
using powerful_crm.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace powerful_crm.Business
{
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public int AddAccount(AccountDto dto)
        {
            return _accountRepository.AddAccount(dto);
        }
        public int DeleteAccount(int id)
        {
            return _accountRepository.DeleteAccount(id);
        }
        public AccountDto GetAccountById(int id)
        {
            return _accountRepository.GetAccountById(id);
        }
    }
}
