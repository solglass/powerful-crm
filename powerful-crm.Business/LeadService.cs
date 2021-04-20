using powerful_crm.Data;
using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;
using powerful_crm.Core.Enums;

namespace powerful_crm.Business
{
    public class LeadService : ILeadService
    {
        private ILeadRepository _leadRepository;

        public LeadService(ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
        }

        public int AddLead(LeadDto dto)
        {
            dto.Password = new SecurityService().GetHash(dto.Password);
            return  _leadRepository.AddUpdateLead(dto);
        }
        public int UpdateLead(int leadId, LeadDto dto)
        {
            dto.Id = leadId;
            return _leadRepository.AddUpdateLead(dto);
        }
        public int DeleteLead(int leadId) => _leadRepository.DeleteOrRecoverLead(leadId, true);
        public int RecoverLead(int leadId) => _leadRepository.DeleteOrRecoverLead(leadId, false);
        public int ChangePassword(int leadId, string oldPassword, string newPassword)
        {
           // var secutity = new SecurityService();
            oldPassword = new SecurityService().GetHash(oldPassword);
            newPassword = new SecurityService().GetHash(newPassword);
            return _leadRepository.ChangePasswordLead(leadId, oldPassword, newPassword);
        }
        public LeadDto GetLeadById(int leadId) => _leadRepository.GetLeadById(leadId);
        public int AddCity(CityDto city) => _leadRepository.AddCity(city);
        public int DeleteCity(int id) => _leadRepository.DeleteCity(id);
        public CityDto GetCityById(int id) => _leadRepository.GetCityById(id);
        public List<LeadDto> SearchLead(SearchLeadDto leadDto)
        {
            if (leadDto.City.Name != null) { leadDto.City.Name = StringWithSearchType.GetStringWithSearchType(leadDto.City.Name, leadDto.TypeSearchCityName); }
            if (leadDto.Email != null) { leadDto.Email = StringWithSearchType.GetStringWithSearchType(leadDto.Email, leadDto.TypeSearchEmail); }
            if (leadDto.Login != null) { leadDto.Login = StringWithSearchType.GetStringWithSearchType(leadDto.Login, leadDto.TypeSearchLogin); }
            if (leadDto.FirstName != null) { leadDto.FirstName = StringWithSearchType.GetStringWithSearchType(leadDto.FirstName, leadDto.TypeSearchFirstName); }
            if (leadDto.LastName != null) { leadDto.LastName = StringWithSearchType.GetStringWithSearchType(leadDto.LastName, leadDto.TypeSearchLastName); }
            if (leadDto.Phone != null) { leadDto.Phone = StringWithSearchType.GetStringWithSearchType(leadDto.Phone, leadDto.TypeSearchPhone); }
            return _leadRepository.SearchLeads(leadDto);
        }
    }
}
