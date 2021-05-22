using powerful_crm.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace powerful_crm.Data
{
    public interface ILeadRepository
    {
        Task<int> AddUpdateLeadAsync(LeadDto dto);
        Task<bool> ChangePasswordLeadAsync(int id, string oldPassword, string newPassword);
        Task<bool> DeleteOrRecoverLeadAsync(int id, bool isDeleted);
        Task<LeadDto> GetLeadByIdAsync(int id);
        Task<List<LeadDto>> SearchLeadsAsync(SearchLeadDto leadDto);
        Task<LeadDto> GetLeadCredentialsAsync(int? id, string login);
        Task<bool> UpdateLeadRoleAsync(int leadId, int roleId);
    }
}