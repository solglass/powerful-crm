﻿using powerful_crm.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
    public interface IAccountService
    {
        Task<int> AddAccountAsync(AccountDto dto);
        Task<bool> DeleteAccountAsync(int id);
        Task<AccountDto> GetAccountByIdAsync(int id);
        Task<List<AccountDto>> GetAccountsByLeadIdAsync(int leadId);
    }
}