using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.MiddleModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Business;
using powerful_crm.Core;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace powerful_crm.API.Controllers
{
    [Route("api/[controller]")]
    public class LeadController : ControllerBase
    {
        private ILeadService _leadService;
        private IMapper _mapper;
        private RestClient _client;

        public LeadController(IOptions<AppSettings> options,IMapper mapper, ILeadService leadService)
        {
            _leadService = leadService;
            _mapper = mapper;
            _client = new RestClient(options.Value.TSTORE_URL);
        }
        /// <summary>Adds new lead</summary>
        /// <param name="inputModel">Information about lead to add</param>
        /// <returns>Information about added lead</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPost]
        public ActionResult<LeadOutputModel> AddLead([FromBody] LeadInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            if (_leadService.GetCityById(inputModel.CityId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_CITYNOTFOUND, inputModel.CityId));
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
        [HttpPut("{leadId}/change-password")]
        public ActionResult ChangePassword(int leadId, [FromBody]ChangePasswordInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            if (_leadService.GetLeadById(leadId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, leadId));
            }
            _leadService.ChangePassword(leadId, inputModel.OldPassword, inputModel.NewPassword);
            return NoContent();
        }
        /// <summary>Gets info about lead</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about lead</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("{leadId}")]
        public ActionResult<LeadOutputModel> GetLead(int leadId)
        {
            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, leadId));
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
        [HttpPut("{leadId}")]
        public ActionResult<LeadOutputModel> UpdateLead(int leadId, [FromBody] UpdateLeadInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, leadId));
            }
            if ( _leadService.GetCityById(inputModel.CityId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_CITYNOTFOUND, inputModel.CityId));
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
        [HttpDelete("{leadId}")]
        public ActionResult<LeadOutputModel> DeleteLead(int leadId)
        {
            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, leadId));
            }
            if (lead.IsDeleted == true)
            {
                return BadRequest(string.Format(Constants.ERROR_LEADALREADYDELETED, leadId));
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
        [HttpPut("{leadId}/recover")]
        public ActionResult<LeadOutputModel> RecoverLead(int leadId)
        {
            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, leadId));
            }
            if (lead.IsDeleted == false)
            {
                return BadRequest(string.Format(Constants.ERROR_LEADNOTDELETED, leadId));
            }
            _leadService.RecoverLead(leadId);
            var dto = _mapper.Map<LeadOutputModel>(_leadService.GetLeadById(leadId));
            return Ok(dto);
        }

        /// <summary>Creates new city</summary>
        /// <param name="city">Information about new city</param>
        /// <returns>Info about created city</returns>
        [ProducesResponseType(typeof(CityOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [HttpPost("city")]
        public ActionResult<CityOutputModel> AddCity([FromBody] CityInputModel city)
        {
            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            var dto = _mapper.Map<CityDto>(city);
            var addedCityId = _leadService.AddCity(dto);
            var outputModel = _mapper.Map<CityOutputModel>(_leadService.GetCityById(addedCityId));
            return Ok(outputModel);
        }

        /// <summary>Deletes the city</summary>
        /// <param name="cityId">Id of the city to delete</param>
        /// <returns>NoContent result</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [HttpDelete("city/{id}")]
        public ActionResult<LeadOutputModel> DeleteCity(int cityId)
        {
            var city = _leadService.GetCityById(cityId);
            if (city == null)
            {
                return NotFound(string.Format(Constants.ERROR_CITYNOTFOUND, cityId));
            }
            if (_leadService.DeleteLead(cityId) == 1)
                return NoContent();
            else
                return Conflict(string.Format(Constants.ERROR_CITYHASDEPENDENCIES, cityId));
        }

        /// <summary>Gets lead balance</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about balance</returns>
        [ProducesResponseType(typeof(List<BalanceOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("{leadId}/balance")]
        public ActionResult<List<BalanceOutputModel>> GetBalanceByLeadId(int leadId)
        {
            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, leadId));
            }

            var request = new RestRequest(string.Format(Constants.API_GETBALANCE, leadId), Method.GET);
            var queryResult = _client.Execute<List<BalanceInputModel>>(request).Data;

            var result = _mapper.Map<List<BalanceOutputModel>>(queryResult);
            return Ok(result);
        }

        /// <summary>Gets lead transactions</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about transactions</returns>
        [ProducesResponseType(typeof(List<TransactionOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpGet("{leadId}/transactions")]
        public ActionResult<List<TransactionOutputModel>> GetTransactionsByLeadId(int leadId)
        {
            var lead = _leadService.GetLeadById(leadId);
            if (lead == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, leadId));
            }

            var request = new RestRequest(string.Format(Constants.API_GETTRANSACTION, leadId), Method.GET);
            var queryResult = _client.Execute<string>(request).Data;

            return Ok(queryResult);
        }

        /// <summary>Adds deposit</summary>
        /// <param name="inputModel">Information about deposit</param>
        /// <returns>Id of added deposit</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPost("deposit")]
        public ActionResult<int> AddDeposit([FromBody] TransactionInputModel inputModel)
        {
            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);
            if (_leadService.GetLeadById(inputModel.LeadId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, inputModel.LeadId));
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
        [HttpPost("withdraw")]
        public ActionResult<int> AddWithdraw([FromBody] TransactionInputModel inputModel)
        {
            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);
            if (_leadService.GetLeadById(inputModel.LeadId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, inputModel.LeadId));
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
        [HttpPost("transfer")]
        public ActionResult<int> AddTransfer([FromBody] TransferInputModel inputModel)
        {
            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);
            if (_leadService.GetLeadById(inputModel.RecipientId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, inputModel.RecipientId));
            }
            if (_leadService.GetLeadById(inputModel.SenderId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, inputModel.SenderId));
            }
            var middle = _mapper.Map<TransferMiddleModel>(inputModel);
            var request = new RestRequest(Constants.API_TRANSFER, Method.POST);
            request.AddParameter("application/json", JsonSerializer.Serialize(middle), ParameterType.RequestBody);
            var queryResult = _client.Execute<int>(request).Data;
            return Ok(queryResult);
        }

    }
}
