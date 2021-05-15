using powerful_crm.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace powerful_crm.Data
{
    public interface ILeadRepository
    {
        Task<int> AddUpdateLeadAsync(LeadDto dto);
        Task<int> ChangePasswordLeadAsync(int id, string oldPassword, string newPassword);
        Task<int> DeleteOrRecoverLeadAsync(int id, bool isDeleted);
        Task<LeadDto> GetLeadByIdAsync(int id);
        Task<List<LeadDto>> SearchLeadsAsync(SearchLeadDto leadDto);
        Task<LeadDto> GetLeadCredentialsAsync(int? id, string login);
        Task<int> UpdateLeadRoleAsync(int leadId, int roleId);
    }
}