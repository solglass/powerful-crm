using powerful_crm.Core.Models;

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
        int AddCity(CityDto city);
        int DeleteCity(int id);
        CityDto GetCityById(int id);
    }
}