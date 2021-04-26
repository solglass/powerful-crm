using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.MiddleModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Business;
using powerful_crm.Core;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.Enums;
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace powerful_crm.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class LeadController : ControllerBase
    {
        private ILeadService _leadService;
        private ICityService _cityService;
        private IMapper _mapper;
        private RestClient _client;

        public LeadController(IOptions<AppSettings> options,IMapper mapper, ILeadService leadService, ICityService cityService)
        {
            _leadService = leadService;
            _cityService = cityService;
            _mapper = mapper;
            _client = new RestClient(options.Value.TSTORE_URL);
        }
        /// <summary>Adds new lead</summary>
        /// <param name="inputModel">Information about lead to add</param>
        /// <returns>Information about added lead</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult<LeadOutputModel> AddLead([FromBody] LeadInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            if (_cityService.GetCityById(inputModel.CityId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_CITY_NOT_FOUND, inputModel.CityId));
            }
            var dto = _mapper.Map<LeadDto>(inputModel);
            var addedLeadId = _leadService.AddLead(dto);
            var outputModel = _mapper.Map<LeadOutputModel>(_leadService.GetLeadById(addedLeadId));
            return Ok(outputModel);

        }
        // https://localhost:44307/api/lead/2/change-password
        /// <summary>Changes password of lead</summary>
        /// <param name="leadId">Id of lead for whom we are changing the password</param>
        /// <param name="inputModel">Old and new password of lead</param>
        /// <returns>NoContent response</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{leadId}/change-password")]
        public ActionResult ChangePassword(int leadId, [FromBody]ChangePasswordInputModel inputModel)
        {
            if (!CheckIfUserIsAllowed(leadId))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);

            if (_leadService.GetLeadById(leadId) == null)
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId));

            _leadService.ChangePassword(leadId, inputModel.OldPassword, inputModel.NewPassword);
            return NoContent();
        }
        /// <summary>Gets info about lead</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about lead</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{leadId}")]
        public ActionResult<LeadOutputModel> GetLead(int leadId)
        {
            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId));
            }

            var outputModel = _mapper.Map<LeadOutputModel>(lead);
            return Ok(outputModel);
        }
        /// <summary>Searches leads by parameters</summary>
        /// <param name="inputModel"> Parameters for searching</param>
        /// <returns>List of leads that suit the parameters</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("search")]
        public ActionResult<List<LeadOutputModel>> SearchLeads ([FromBody] SearchLeadInputModel inputModel)
        {

            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            var dto = _mapper.Map<SearchLeadDto>(inputModel);
            var leads = _leadService.SearchLead(dto);
            if (leads.Count==0)
            {
                return NotFound($"Leads is not found");
            }
            var outputModels = _mapper.Map<List<LeadOutputModel>>(leads);
            return Ok(outputModels);
        }
        /// <summary>Updates information about lead</summary>
        /// <param name="leadId">Id of lead</param>
        /// <param name="inputModel">Updated info about lead</param>
        /// <returns>Updated info about lead</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{leadId}")]
        public ActionResult<LeadOutputModel> UpdateLead(int leadId, [FromBody] UpdateLeadInputModel inputModel)
        {
            if (!CheckIfUserIsAllowed(leadId))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId));
            }
            if ( _cityService.GetCityById(inputModel.CityId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_CITY_NOT_FOUND, inputModel.CityId));
            }
            var dto = _mapper.Map<LeadDto>(inputModel);
            _leadService.UpdateLead(leadId, dto);
            var outputModel = _mapper.Map<LeadOutputModel>(_leadService.GetLeadById(leadId));
            return Ok(outputModel);

        }

        /// <summary>Changes value of parameter "IsDeleted" to 1(Deleted)</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about lead which is deleted</returns>
        [ProducesResponseType(typeof(List<LeadOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("{leadId}")]
        public ActionResult<LeadOutputModel> DeleteLead(int leadId)
        {
            if (!CheckIfUserIsAllowed(leadId))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId));
            }
            if (lead.IsDeleted == true)
            {
                return BadRequest(string.Format(Constants.ERROR_LEAD_ALREADY_DELETED, leadId));
            }
            _leadService.DeleteLead(leadId);
            var dto = _mapper.Map<LeadOutputModel>(_leadService.GetLeadById(leadId));
            return Ok(dto);
        }

        /// <summary>Changes value of parameter "IsDeleted" to 0(Not deleted)</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about lead which is recovered</returns>
        [ProducesResponseType(typeof(List<LeadOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPut("{leadId}/recover")]
        public ActionResult<LeadOutputModel> RecoverLead(int leadId)
        {
            if (!CheckIfUserIsAllowed(leadId))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId));
            }
            if (lead.IsDeleted == false)
            {
                return BadRequest(string.Format(Constants.ERROR_LEAD_NOT_DELETED, leadId));
            }
            _leadService.RecoverLead(leadId);
            var dto = _mapper.Map<LeadOutputModel>(_leadService.GetLeadById(leadId));
            return Ok(dto);
        }

       
        /// <summary>Gets lead balance</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about balance</returns>
        [ProducesResponseType(typeof(List<BalanceOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{leadId}/balance")]
        public ActionResult<List<BalanceOutputModel>> GetBalanceByLeadId(int leadId)
        {
            if (!CheckIfUserIsAllowed(leadId))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId));
            }

            var request = new RestRequest(string.Format(Constants.API_GET_BALANCE, leadId), Method.GET);
            var queryResult = _client.Execute<List<BalanceInputModel>>(request).Data;

            var result = _mapper.Map<List<BalanceOutputModel>>(queryResult);
            return Ok(result);
        }

        /// <summary>Gets lead transactions</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about transactions</returns>
        [ProducesResponseType(typeof(List<TransactionOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{leadId}/transactions")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionsByLeadId(int leadId)
        {
            if (!CheckIfUserIsAllowed(leadId))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId));
            }

            var request = new RestRequest(string.Format(Constants.API_GET_TRANSACTION, leadId), Method.GET);
            var queryResult = _client.Execute<string>(request).Data;

            return Ok(queryResult);
        }

        /// <summary>Adds deposit</summary>
        /// <param name="inputModel">Information about deposit</param>
        /// <returns>Id of added deposit</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("deposit")]
        public ActionResult<int> AddDeposit([FromBody] TransactionInputModel inputModel)
        {
            if (!CheckIfUserIsAllowed(inputModel.LeadId))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);
            if (_leadService.GetLeadById(inputModel.LeadId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.LeadId));
            }
            var middle = _mapper.Map<TransactionMiddleModel>(inputModel);
            var request = new RestRequest(Constants.API_DEPOSIT, Method.POST);
            request.AddParameter("application/json", JsonSerializer.Serialize(middle), ParameterType.RequestBody);
            var queryResult = _client.Execute<int>(request).Data;
            return Ok(queryResult);
        }

        /// <summary>Adds withdraw</summary>
        /// <param name="inputModel">Information about withdraw</param>
        /// <returns>Id of added withdraw</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("withdraw")]
        public ActionResult<int> AddWithdraw([FromBody] TransactionInputModel inputModel)
        {
           if (!CheckIfUserIsAllowed(inputModel.LeadId))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);
            if (_leadService.GetLeadById(inputModel.LeadId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.LeadId));
            }
            var middle = _mapper.Map<TransactionMiddleModel>(inputModel);
            var request = new RestRequest(Constants.API_WITHDRAW, Method.POST);
            request.AddParameter("application/json", JsonSerializer.Serialize(middle), ParameterType.RequestBody);
            var queryResult = _client.Execute<int>(request).Data;
            return Ok(queryResult);
        }

        /// <summary>Adds transfer</summary>
        /// <param name="inputModel">Information about transfer</param>
        /// <returns>Id of added transfer</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("transfer")]
        public ActionResult<int> AddTransfer([FromBody] TransferInputModel inputModel)
        {
            if (!CheckIfUserIsAllowed(inputModel.SenderId))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);

            if (_leadService.GetLeadById(inputModel.RecipientId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.RecipientId));
            }
            if (_leadService.GetLeadById(inputModel.SenderId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.SenderId));
            }
            var middle = _mapper.Map<TransferMiddleModel>(inputModel);
            var request = new RestRequest(Constants.API_TRANSFER, Method.POST);
            request.AddParameter("application/json", JsonSerializer.Serialize(middle), ParameterType.RequestBody);
            var queryResult = _client.Execute<int>(request).Data;
            return Ok(queryResult);
        }

        private bool CheckIfUserIsAllowed(int leadId)
        {
            return leadId.ToString() == HttpContext.User.Claims.Where(t=>t.Type==ClaimTypes.NameIdentifier).FirstOrDefault().Value 
                || HttpContext.User.IsInRole(Role.Administrator.ToString());
        }
    }
}
