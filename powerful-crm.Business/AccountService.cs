using powerful_crm.Core.Models;
using powerful_crm.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;
        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }
        public async Task<int> DeleteAccountAsync(int id) => await _accountRepository.DeleteAccountAsync(id);
        public async Task<int> AddAccountAsync(AccountDto dto) => await _accountRepository.AddAccountAsync(dto);
        public async Task<AccountDto> GetAccountByIdAsync(int id) => await _accountRepository.GetAccountByIdAsync(id);
        public async Task<List<AccountDto>> GetAccountsByLeadIdAsync(int leadId) => await _accountRepository.GetAccountsByLeadIdAsync(leadId);
    }
}
