using powerful_crm.Core.Models;
using System.Collections.Generic;

namespace powerful_crm.Data
{
    public interface ILeadRepository
    {
        int AddCity(CityDto dto);
        int AddLead(LeadDto dto);
        int ChangePasswordLead(int id, string oldPassword, string newPassword);
        int DeleteCity(int id);
        int DeleteOrRecoverLead(int id, bool isDeleted);
        LeadDto GetLeadById(int id);
        List<LeadDto> GetLeadsByEmail(string email);
        List<LeadDto> GetLeadsByFirstName(string firstName);
        List<LeadDto> GetLeadsByIsDeleted(bool isDeleted);
        List<LeadDto> GetLeadsByLastName(string lastName);
        List<LeadDto> GetLeadsByLogin(string login);
        List<LeadDto> GetLeadsByPhone(string phone);
        int UpdateLead(LeadDto dto);
    }
}