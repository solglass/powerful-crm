using powerful_crm.Data;
using powerful_crm.Core.Models;
using System;

namespace powerful_crm.Business
{
    public class LeadService : ILeadService
    {
        private ILeadRepository _leadRepository;

        public LeadService(ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
        }

        public int AddLead(LeadDto dto) => _leadRepository.AddUpdateLead(dto);
        public int UpdateLead(int leadId, LeadDto dto)
        {
            dto.Id = leadId;
            return _leadRepository.AddUpdateLead(dto);
        }
        public int DeleteLead(int leadId) => _leadRepository.DeleteOrRecoverLead(leadId, true);
        public int RecoverLead(int leadId) => _leadRepository.DeleteOrRecoverLead(leadId, false);
        public int ChangePassword(int leadId, string oldPassword, string newPassword) => _leadRepository.ChangePasswordLead(leadId, oldPassword, newPassword);
        public LeadDto GetLeadById(int leadId) => _leadRepository.GetLeadById(leadId);
        public int AddCity(CityDto city) => _leadRepository.AddCity(city);
        public int DeleteCity(int id) => _leadRepository.DeleteCity(id);
        public CityDto GetCityById(int id) => _leadRepository.GetCityById(id);
    }
}
