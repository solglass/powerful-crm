using powerful_crm.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
    public interface ILeadService
    {
        Task<int> AddLeadAsync(LeadDto dto);
        Task<int> ChangePasswordAsync(int leadId, string oldPassword, string newPassword);
        Task<int> DeleteLeadAsync(int leadId);
        Task<LeadDto> GetLeadByIdAsync(int leadId);
        Task<List<LeadDto>> SearchLeadAsync(SearchLeadDto leadDto);
        Task<int> RecoverLeadAsync(int leadId);
        Task<int> UpdateLeadAsync(int leadId, LeadDto dto);
        Task<int> UpdateLeadRoleAsync(int leadId, int roleId);
    }
}