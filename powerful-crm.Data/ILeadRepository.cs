using powerful_crm.Core.Models;
using System;
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
        List<LeadDto> SearchLeads(string? firstName, string? lastName, string? email, string? login, string? phone, DateTime? birthDate, string? cityName);
        int UpdateLead(LeadDto dto);
    }
}