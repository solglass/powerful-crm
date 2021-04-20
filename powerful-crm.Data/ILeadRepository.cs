using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;

namespace powerful_crm.Data
{
    public interface ILeadRepository
    {
        int AddUpdateLead(LeadDto dto);
        int ChangePasswordLead(int id, string oldPassword, string newPassword);
        int DeleteCity(int id);
        int DeleteOrRecoverLead(int id, bool isDeleted);
        LeadDto GetLeadById(int id);
        List<LeadDto> SearchLeads(SearchLeadDto leadDto);
        int AddCity(CityDto name);
        CityDto GetCityById(int id);
        LeadDto GetLeadCredentials(int id, string login);

    }
}