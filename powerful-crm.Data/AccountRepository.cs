using Dapper;
using Microsoft.Extensions.Options;
using powerful_crm.Core;
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.Data
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        public AccountRepository(IOptions<AppSettings> options) : base(options)
        {
            _connection = new SqlConnection(_connectionString);
        }
        public async Task<int> AddAccountAsync(AccountDto dto)
        {
            return await _connection.QuerySingleOrDefaultAsync<int>(
                "dbo.Account_Add",
                param: new
                {
                    name = dto.Name,
                    currency = (int)dto.Currency,
                    leadId = dto.LeadDto.Id,
                },
                commandType: CommandType.StoredProcedure);
        }
        public async Task<bool> DeleteAccountAsync(int id)
        {
          return  (await _connection
                .ExecuteAsync("dbo.Account_Delete",
                new { id },
                commandType: CommandType.StoredProcedure)) == Constants.EXPECTED_CHANGED_ROWS_COUNT;
            
        }
        public async Task<AccountDto> GetAccountByIdAsync(int id)
        {
            return (await _connection.QueryAsync<AccountDto, LeadDto, AccountDto>(
                "dbo.Account_SelectById", (account, lead) =>
                {
                    account.LeadDto = lead;
                    return account;
                },
                new { id },
                splitOn: "Id", commandType: CommandType.StoredProcedure)).FirstOrDefault();
        }
        public async Task<List<AccountDto>> GetAccountsByLeadIdAsync(int leadId)
        {
            return (await _connection.QueryAsync<AccountDto, LeadDto, AccountDto>(
                "dbo.Account_SelectByLeadId", (account, lead) =>
                 {
                     account.LeadDto = lead;
                     return account;
                 },
                new { leadId },
                splitOn: "Id", commandType: CommandType.StoredProcedure))
                .Distinct().ToList();
        }
    }
}
