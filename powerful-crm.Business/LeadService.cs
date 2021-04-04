using powerful_crm.Data;
using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;

namespace powerful_crm.Business
{
    public class LeadService : ILeadService
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
        public List<LeadDto> GetLeadsByEmail(string email) => _leadRepository.GetLeadsByEmail(email);
        public List<LeadDto> GetLeadsByFirstName(string firstName) => _leadRepository.GetLeadsByFirstName(firstName);
        public List<LeadDto> GetLeadsByLastName(string lastName) => _leadRepository.GetLeadsByLastName(lastName);
        public List<LeadDto> GetLeadsByLogin(string login) => _leadRepository.GetLeadsByLogin(login);
        public List<LeadDto> GetLeadsByPhone(string phone) => _leadRepository.GetLeadsByPhone(phone);
        public List<LeadDto> GetLeadsByIsDeleted(bool isDeleted) => _leadRepository.GetLeadsByIsDeleted(isDeleted);
        public List<LeadDto> GetLeadsByCity(string city) => _leadRepository.GetLeadsByCity(city);
        public List<LeadDto> GetLeadsByBirthDate(DateTime birthDate) => _leadRepository.GetLeadsByBirthDate(birthDate);
    }
}
