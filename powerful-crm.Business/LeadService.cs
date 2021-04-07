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
        public List<LeadDto> SearchLead(SearchLeadDto leadDto)
        {
            leadDto.City.Name = GetFriendlyString.GetFriendlyType(leadDto?.City.Name, leadDto.TypeSearchCityName);
            leadDto.Email = GetFriendlyString.GetFriendlyType(leadDto?.Email, leadDto.TypeSearchEmail);
            leadDto.Login = GetFriendlyString.GetFriendlyType(leadDto?.Login, leadDto.TypeSearchLogin);
            leadDto.FirstName = GetFriendlyString.GetFriendlyType(leadDto?.FirstName, leadDto.TypeSearchFirstName);
            leadDto.LastName = GetFriendlyString.GetFriendlyType(leadDto?.LastName, leadDto.TypeSearchLastName);
            leadDto.Phone = GetFriendlyString.GetFriendlyType(leadDto?.Phone, leadDto.TypeSearchPhone);
            return _leadRepository.SearchLeads(leadDto);
        }
    }
}
