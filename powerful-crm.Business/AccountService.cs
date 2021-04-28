using powerful_crm.Core.Models;
using powerful_crm.Data;
using System.Collections.Generic;

namespace powerful_crm.Business
{
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public int DeleteAccount(int id) => _accountRepository.DeleteAccount(id);
        public int AddAccount(AccountDto dto) => _accountRepository.AddAccount(dto);
        public AccountDto GetAccountById(int id) => _accountRepository.GetAccountById(id);
        public List<AccountDto> GetAccountsByLeadId(int leadId) => _accountRepository.GetAccountsByLeadId(leadId);
    }
}
