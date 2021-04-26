using powerful_crm.Data;
using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;
using powerful_crm.Core.Enums;
using powerful_crm.Core.CustomExceptions;

namespace powerful_crm.Business
{
    public class LeadService : ILeadService
    {
        private ILeadRepository _leadRepository;
        private ISecurityService _securityService;
        public LeadService(ILeadRepository leadRepository, ISecurityService securityService)
        {
            _leadRepository = leadRepository;
            _securityService = securityService;
        }

        public int AddLead(LeadDto dto)
        {
            dto.Password = _securityService.GetHash(dto.Password);
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
            if (_securityService.VerifyPassword(_leadRepository.GetLeadCredentials(leadId, null).Password, oldPassword))
            {
                newPassword = _securityService.GetHash(newPassword);
                if(_leadRepository.ChangePasswordLead(leadId, oldPassword, newPassword)==1)
                    return 1;
                throw new Exception();
            }
            throw new WrongCredentialsException();
        }
        public LeadDto GetLeadById(int leadId) => _leadRepository.GetLeadById(leadId);
        public int UpdateLeadRole(int leadId, int roleId) => _leadRepository.UpdateLeadRole(leadId, roleId);
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
