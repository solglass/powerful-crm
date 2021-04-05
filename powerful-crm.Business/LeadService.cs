using powerful_crm.Data;
using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;

namespace powerful_crm.Business
{
    public class LeadService :ILeadService
    {
        private ILeadRepository _leadRepository;

        public LeadService(ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
        }

        public int AddLead(LeadDto dto) => _leadRepository.AddLead(dto);
        public int UpdateLead(int leadId, LeadDto dto)
        {
            dto.Id = leadId;
            return _leadRepository.UpdateLead(dto);
        }
        public int DeleteLead(int leadId) => _leadRepository.DeleteOrRecoverLead(leadId, true);
        public int RecoverLead(int leadId) => _leadRepository.DeleteOrRecoverLead(leadId, false);
        public int ChangePassword(int leadId, string oldPassword, string newPassword) => _leadRepository.ChangePasswordLead(leadId, oldPassword, newPassword);
        public LeadDto GetLeadById(int leadId) => _leadRepository.GetLeadById(leadId);
        public List<LeadDto> GetLeadsByEmail(string email) => _leadRepository.SearchLeads(null, null, email, null, null, null, null);
        public List<LeadDto> GetLeadsByFirstName(string firstName) => _leadRepository.SearchLeads(firstName, null, null, null, null, null, null);
        public List<LeadDto> GetLeadsByLastName(string lastName) => _leadRepository.SearchLeads(null, lastName, null, null, null, null, null);
        public List<LeadDto> GetLeadsByLogin(string login) => _leadRepository.SearchLeads(null, null, null, login, null, null, null);
        public List<LeadDto> GetLeadsByPhone(string phone) => _leadRepository.SearchLeads(null, null, null, null, phone, null, null);
        public List<LeadDto> GetLeadsByCity(string city) => _leadRepository.SearchLeads(null, null, null, null, null, null, city);
        public List<LeadDto> GetLeadsByBirthDate(DateTime birthDate) => _leadRepository.SearchLeads(null, null, null, null, null, birthDate, null);
    }
}
