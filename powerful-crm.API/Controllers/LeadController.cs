using AutoMapper;
using Google.Authenticator;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Business;
using powerful_crm.Core;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Security.Claims;

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
        private IPublishEndpoint _publishEndpoint;
        private MemoryCacheSingleton _validatedModelCache;

        public LeadController(IMapper mapper,
                              ILeadService leadService,
                              ICityService cityService,
                              Checker checker,
                              IPublishEndpoint publishEndpoint)
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
            var lead = _leadService.GetLeadById(leadId);

            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            var setupInfo = twoFactor.GenerateSetupCode("myapp", lead.Email, TwoFactorKey(lead.Email), false, 3);

            await _publishEndpoint.Publish<SetupCode>(new
            {
                Value = setupInfo
            });
            return setupInfo;
        }


        /// <summary>Adds withdraw</summary>
        /// <param name="inputModel">Information about withdraw</param>
        /// <returns>Key of validated and added to MemoryCache input model </returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("withdraw")]
        public ActionResult<int> AddWithdraw([FromBody] TransactionInputModel inputModel)
        {
            if (!_checker.CheckIfUserIsAllowed(inputModel.LeadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);
            var lead = _leadService.GetLeadById(inputModel.LeadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.LeadId));
            }

            var memoryCacheKey = (int)DateTime.Now.Ticks;
            _validatedModelCache.Cache.Set<TransactionInputModel>(memoryCacheKey, inputModel);
            
            return Ok(memoryCacheKey);
        }

        /// <summary>Provide withdraw into DB if user confirm operation by GA</summary>
        /// <param name="memoryCacheKey">key to ValidatedInputModels Dictionary</param>
        /// <param name="inputCode">Code from Google Authentificator</param>
        /// <returns>Id of added withdraw</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("providewithdraw")]
        public ActionResult<int> ProvideWithdraw(int memoryCacheKey, string inputCode)
        {

            if (!_validatedModelCache.Cache.TryGetValue(memoryCacheKey, out TransactionInputModel inputModel))
            {
                return NotFound("operation not found");
            }

            if (!_checker.CheckIfUserIsAllowed(inputModel.LeadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);
            
            var lead = _leadService.GetLeadById(inputModel.LeadId);

            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactorKey(lead.Email), inputCode);
            if (!isValid)
                throw new ForbidException("Operation not confirmed");

            var middle = _mapper.Map<TransactionMiddleModel>(inputModel);
            var request = new RestRequest(Constants.API_WITHDRAW, Method.POST);
            request.AddParameter("application/json", JsonSerializer.Serialize(middle), ParameterType.RequestBody);
            var queryResult = _client.Execute<int>(request).Data;

            _validatedModelCache.Cache.Remove(memoryCacheKey);

            return Ok(queryResult);
        }

        private static string TwoFactorKey(string email)
        {
            return $"myverysecretkey+{email}";
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
