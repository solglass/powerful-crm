using powerful_crm.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace powerful_crm.Data
{
    public interface IAccountRepository
    {
        Task<int> AddAccountAsync(AccountDto dto);
        Task<int> DeleteAccountAsync(int id);
        Task<AccountDto> GetAccountByIdAsync(int id);
        Task<List<AccountDto>> GetAccountsByLeadIdAsync(int leadId);
    }
}