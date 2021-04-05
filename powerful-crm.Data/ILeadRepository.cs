﻿using powerful_crm.Core.Models;

namespace powerful_crm.Data
{
    public interface ILeadRepository
    {
        int AddLead(LeadDto dto);
        int ChangePasswordLead(int id, string oldPassword, string newPassword);
        int DeleteOrRecoverLead(int id, bool isDeleted);
        LeadDto GetLeadById(int id);
        int UpdateLead(LeadDto dto);
        int AddCity(CityDto name);
        int DeleteCity(int id);
        CityDto GetCityById(int id);

    }
}