using AutoMapper;
using Google.Authenticator;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Business;
using powerful_crm.Core;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;
using EventContracts;

namespace powerful_crm.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class LeadController : ControllerBase
    {
        private ILeadService _leadService;
        private ICityService _cityService;
        private Checker _checker;
        private IMapper _mapper;
        private IBusControl _publishEndpoint;
        private MemoryCacheSingleton _validatedModelCache;

        public LeadController(IMapper mapper,
                              ILeadService leadService,
                              ICityService cityService,
                              Checker checker,
                              IBusControl publishEndpoint)
        {
            _leadService = leadService;
            _checker = checker;
            _cityService = cityService;
            _mapper = mapper;
            _validatedModelCache = MemoryCacheSingleton.GetCacheInstance();
            _publishEndpoint = publishEndpoint;
        }
        /// <summary>GetManualGA Code</summary>
        /// <returns>Manual GA setup code</returns>
        [HttpGet("getcode")]
        public async Task<ActionResult<SetupCode>> GetCodeAsync()
        {
            var leadId= Convert.ToInt32(HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value);
            var lead = await _leadService.GetLeadByIdAsync(leadId);

            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            var setupInfo = twoFactor.GenerateSetupCode("myapp", lead.Email, TwoFactor.TwoFactorKey(lead.Email), false, 3);

            var setupInfoDictionary = new Dictionary<string, string>();
            setupInfoDictionary.Add("Account", setupInfo.Account);
            setupInfoDictionary.Add("ManualEntryKey", setupInfo.ManualEntryKey);
            setupInfoDictionary.Add("QrCodeSetupImageUrl", setupInfo.QrCodeSetupImageUrl);
            await _publishEndpoint.Publish<SetupCodeInfo>(
            
                new SetupCodeInfo { SendValue = setupInfoDictionary }
            );
            return setupInfo;
        }

        /// <summary>Adds new lead</summary>
        /// <param name="inputModel">Information about lead to add</param>
        /// <returns>Information about added lead</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LeadOutputModel>> AddLeadAsync([FromBody] LeadInputModel inputModel)
        {
            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);
            
            if (await _cityService.GetCityByIdAsync(inputModel.CityId) == null)
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_CITY_NOT_FOUND, inputModel.CityId)
                });
            
            var dto = _mapper.Map<LeadDto>(inputModel);
            var addedLeadId = await _leadService.AddLeadAsync(dto);
            var outputModel = _mapper.Map<LeadOutputModel>(await _leadService.GetLeadByIdAsync(addedLeadId));
            return Ok(outputModel);

        }
        // https://localhost:44307/api/lead/2/change-password
        /// <summary>Changes password of lead</summary>
        /// <param name="leadId">Id of lead for whom we are changing the password</param>
        /// <param name="inputModel">Old and new password of lead</param>
        /// <returns>NoContent response</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{leadId}/change-password")]
        public async Task<ActionResult> ChangePasswordAsync(int leadId, [FromBody] ChangePasswordInputModel inputModel)
        {
            if (!_checker.CheckIfUserIsAllowed(leadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);

            if (await _leadService.GetLeadByIdAsync(leadId) == null)
                return NotFound(new CustomExceptionOutputModel
                {
                    Code= StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId)
                });

            await _leadService.ChangePasswordAsync(leadId, inputModel.OldPassword, inputModel.NewPassword);
            return NoContent();
        }
        /// <summary>Gets info about lead</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about lead</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{leadId}")]
        public async Task<ActionResult<LeadOutputModel>> GetLeadAsync(int leadId)
        {
            var lead = await _leadService.GetLeadByIdAsync(leadId);
            if (lead == null)
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId)
                });
            
            var outputModel = _mapper.Map<LeadOutputModel>(lead);
            return Ok(outputModel);
        }
        /// <summary>Searches leads by parameters</summary>
        /// <param name="inputModel"> Parameters for searching</param>
        /// <returns>List of leads that suit the parameters</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("search")]
        public async Task<ActionResult<List<LeadOutputModel>>> SearchLeadsAsync([FromBody] SearchLeadInputModel inputModel)
        {

            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            var dto = _mapper.Map<SearchLeadDto>(inputModel);
            var leads = await _leadService.SearchLeadAsync(dto);

            if (leads.Count == 0)
                return NotFound( new CustomExceptionOutputModel 
                { 
                    Code = StatusCodes.Status404NotFound,
                    Message = Constants.ERROR_LEADS_NOT_FOUND
                });
            
            var outputModels = _mapper.Map<List<LeadOutputModel>>(leads);
            return Ok(outputModels);
        }
        /// <summary>Updates information about lead</summary>
        /// <param name="leadId">Id of lead</param>
        /// <param name="inputModel">Updated info about lead</param>
        /// <returns>Updated info about lead</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{leadId}")]
        public async Task<ActionResult<LeadOutputModel>> UpdateLeadAsync([FromRoute] int leadId, [FromBody] UpdateLeadInputModel inputModel)
        {
            if (!_checker.CheckIfUserIsAllowed(leadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);
            
            var lead = await _leadService.GetLeadByIdAsync(leadId);
            if (lead == null)
                 return NotFound(new CustomExceptionOutputModel
                 {
                     Code = StatusCodes.Status404NotFound,
                     Message = string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId)
                 });
            
            if (await _cityService.GetCityByIdAsync(inputModel.CityId) == null)
                 return NotFound(new CustomExceptionOutputModel
                 {
                     Code = StatusCodes.Status404NotFound,
                     Message = string.Format(Constants.ERROR_CITY_NOT_FOUND, inputModel.CityId)
                 });
            
            var dto = _mapper.Map<LeadDto>(inputModel);
            await _leadService.UpdateLeadAsync(leadId, dto);
            var outputModel = _mapper.Map<LeadOutputModel>(await _leadService.GetLeadByIdAsync(leadId));
            return Ok(outputModel);

        }

        /// <summary>Changes value of parameter "IsDeleted" to 1(Deleted)</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about lead which is deleted</returns>
        [ProducesResponseType(typeof(List<LeadOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("{leadId}")]
        public async Task<ActionResult<LeadOutputModel>> DeleteLeadAsync(int leadId)
        {
            if (!_checker.CheckIfUserIsAllowed(leadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            var lead = await _leadService.GetLeadByIdAsync(leadId);
            if (lead == null)
                return NotFound(new CustomExceptionOutputModel 
                { 
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId) 
                });
            
            if (lead.IsDeleted)
                return Conflict(new CustomExceptionOutputModel
                {
                    Code= StatusCodes.Status409Conflict,
                    Message=  string.Format(Constants.ERROR_LEAD_ALREADY_DELETED, leadId)
                });
            
            await _leadService.DeleteLeadAsync(leadId);
            var dto = _mapper.Map<LeadOutputModel>(await _leadService.GetLeadByIdAsync(leadId));
            return Ok(dto);
        }

        /// <summary>Changes value of parameter "IsDeleted" to 0(Not deleted)</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about lead which is recovered</returns>
        [ProducesResponseType(typeof(List<LeadOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{leadId}/recover")]
        public async Task<ActionResult<LeadOutputModel>> RecoverLeadAsync(int leadId)
        {
            if (!_checker.CheckIfUserIsAllowed(leadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            var lead = await _leadService.GetLeadByIdAsync(leadId);
            if (lead == null)
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId)
                });
            if (!lead.IsDeleted)
                return Conflict(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status409Conflict,
                    Message = string.Format(Constants.ERROR_LEAD_NOT_DELETED, leadId)
                });

            await _leadService.RecoverLeadAsync(leadId);
            var dto = _mapper.Map<LeadOutputModel>(await _leadService.GetLeadByIdAsync(leadId));
            return Ok(dto);
        }


       
    }
}
