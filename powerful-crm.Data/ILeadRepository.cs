using powerful_crm.Core.Models;
using System.Collections.Generic;

namespace powerful_crm.Data
{
    public interface ILeadRepository
    {
        int AddUpdateLead(LeadDto dto);
        int ChangePasswordLead(int id, string oldPassword, string newPassword);
        int DeleteOrRecoverLead(int id, bool isDeleted);
        LeadDto GetLeadById(int id);
        List<LeadDto> SearchLeads(SearchLeadDto leadDto);
        LeadDto GetLeadCredentials(int? id, string login);
        int UpdateLeadRole(int leadId, int roleId);
    }
}