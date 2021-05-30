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
        private IAccountService _accountService;
        public LeadService(ILeadRepository leadRepository, ISecurityService securityService, IAccountRepository accountRepository, IAccountService accountService)
        {
            _leadRepository = leadRepository;
            _accountService = accountService;
            _accountRepository = accountRepository;
            _securityService = securityService;
        }

        public async Task<int> AddLeadAsync(LeadDto dto)
        {
            dto.Password =  _securityService.GetHash(dto.Password);
            var addedLeadId = await _leadRepository.AddUpdateLeadAsync(dto);
            await _accountService.AddAccountAsync(new AccountDto { Name = "Default", Currency = (Currency)1, LeadDto = new LeadDto() { Id = addedLeadId } });
            return addedLeadId;
        }
        public async Task<int> UpdateLeadAsync(int leadId, LeadDto dto)
        {
            dto.Id = leadId;
            return await _leadRepository.AddUpdateLeadAsync(dto);
        }
        public async Task<bool> DeleteLeadAsync(int leadId) => await _leadRepository.DeleteOrRecoverLeadAsync(leadId, true);
        public async Task<bool> RecoverLeadAsync(int leadId) => await _leadRepository.DeleteOrRecoverLeadAsync(leadId, false);
        public async Task<bool> ChangePasswordAsync(int leadId, string oldPassword, string newPassword)
        {
            if (_securityService.VerifyPassword((await _leadRepository.GetLeadCredentialsAsync(leadId, null)).Password, oldPassword))
            {
                newPassword = _securityService.GetHash(newPassword);
                return await _leadRepository.ChangePasswordLeadAsync(leadId, oldPassword, newPassword);
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
        public async Task<bool> UpdateLeadRoleAsync(int leadId, int roleId) => await _leadRepository.UpdateLeadRoleAsync(leadId, roleId);
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
