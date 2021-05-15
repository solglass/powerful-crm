using powerful_crm.Data;
using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;
using powerful_crm.Core.Enums;
using powerful_crm.Core.CustomExceptions;
using System.Threading.Tasks;

namespace powerful_crm.Business
{
    public class LeadService : ILeadService
    {
        private ILeadRepository _leadRepository;
        private ISecurityService _securityService;
        private IAccountRepository _accountRepository;
        public LeadService(ILeadRepository leadRepository, ISecurityService securityService, IAccountRepository accountRepository)
        {
            _leadRepository = leadRepository;
            _accountRepository = accountRepository;
            _securityService = securityService;
        }

        public async Task<int> AddLeadAsync(LeadDto dto)
        {
            dto.Password =  _securityService.GetHash(dto.Password);
            return await _leadRepository.AddUpdateLeadAsync(dto);
        }
        public async Task<int> UpdateLeadAsync(int leadId, LeadDto dto)
        {
            dto.Id = leadId;
            return await _leadRepository.AddUpdateLeadAsync(dto);
        }
        public async Task<int> DeleteLeadAsync(int leadId) => await _leadRepository.DeleteOrRecoverLeadAsync(leadId, true);
        public async Task<int> RecoverLeadAsync(int leadId) => await _leadRepository.DeleteOrRecoverLeadAsync(leadId, false);
        public async Task<int> ChangePasswordAsync(int leadId, string oldPassword, string newPassword)
        {
            if (_securityService.VerifyPassword((await _leadRepository.GetLeadCredentialsAsync(leadId, null)).Password, oldPassword))
            {
                newPassword = _securityService.GetHash(newPassword);
                if(await _leadRepository.ChangePasswordLeadAsync(leadId, oldPassword, newPassword)==1)
                    return 1;
                throw new Exception();
            }
            throw new WrongCredentialsException();
        }
        public async Task<LeadDto> GetLeadByIdAsync(int leadId)
        {
          var lead =  await _leadRepository.GetLeadByIdAsync(leadId);
            if(lead!=null)
            {
                lead.Accounts = await _accountRepository.GetAccountsByLeadIdAsync(leadId);
            }
            return lead;

        }
        public async Task<int> UpdateLeadRoleAsync(int leadId, int roleId) => await _leadRepository.UpdateLeadRoleAsync(leadId, roleId);
        public async Task<List<LeadDto>> SearchLeadAsync(SearchLeadDto leadDto)
        {
            if (leadDto.City.Name != null) { leadDto.City.Name = StringWithSearchType.GetStringWithSearchType(leadDto.City.Name, leadDto.TypeSearchCityName); }
            if (leadDto.Email != null) { leadDto.Email = StringWithSearchType.GetStringWithSearchType(leadDto.Email, leadDto.TypeSearchEmail); }
            if (leadDto.Login != null) { leadDto.Login = StringWithSearchType.GetStringWithSearchType(leadDto.Login, leadDto.TypeSearchLogin); }
            if (leadDto.FirstName != null) { leadDto.FirstName = StringWithSearchType.GetStringWithSearchType(leadDto.FirstName, leadDto.TypeSearchFirstName); }
            if (leadDto.LastName != null) { leadDto.LastName = StringWithSearchType.GetStringWithSearchType(leadDto.LastName, leadDto.TypeSearchLastName); }
            if (leadDto.Phone != null) { leadDto.Phone = StringWithSearchType.GetStringWithSearchType(leadDto.Phone, leadDto.TypeSearchPhone); }
            return await _leadRepository.SearchLeadsAsync(leadDto);
        }
    }
}
