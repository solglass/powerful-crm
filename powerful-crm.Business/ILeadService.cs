using powerful_crm.Core.Models;
using System.Collections.Generic;

namespace powerful_crm.Business
{
    public interface ILeadService
    {
        int AddLead(LeadDto dto);
        int ChangePassword(int leadId, string oldPassword, string newPassword);
        int DeleteLead(int leadId);
        LeadDto GetLeadById(int leadId);
        List<LeadDto> SearchLead(SearchLeadDto leadDto);
        int RecoverLead(int leadId);
        int UpdateLead(int leadId, LeadDto dto);
        int UpdateLeadRole(int leadId, int roleId);
    }
}