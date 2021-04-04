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
        int RecoverLead(int leadId);
        int UpdateLead(int leadId, LeadDto dto);
        List<LeadDto> GetLeadsByEmail(string email);
        List<LeadDto> GetLeadsByFirstName(string firstName);
        List<LeadDto> GetLeadsByLastName(string lastName);
        List<LeadDto> GetLeadsByLogin(string login);
        List<LeadDto> GetLeadsByPhone(string phone);
        List<LeadDto> GetLeadsByIsDeleted(bool isDeleted);
    }
}