using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Business;
using powerful_crm.Core;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;
using RestSharp;
using System.Collections.Generic;

namespace powerful_crm.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        /// <summary>lead add</summary>
        /// <param name="inputModel">information about add lead</param>
        /// <returns>rReturn information about added lead</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPost]
        public ActionResult<LeadOutputModel> AddLead([FromBody] LeadInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ModelState);
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
        /// <summary>Changing password of lead</summary>
        /// <param name="leadId">Id of lead for whom we are changing the password</param>
        /// <param name="inputModel">Old and new password of lead</param>
        /// <returns>Status204NoContent response</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [HttpPut("{leadId}/change-password")]
        public ActionResult ChangePassword(int leadId, [FromBody]ChangePasswordInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ModelState);
            }
            if (_leadService.GetLeadById(leadId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEADNOTFOUND, leadId));
            }
            _leadService.ChangePassword(leadId, inputModel.OldPassword, inputModel.NewPassword);
            return NoContent();
        }
        /// <summary>Get info of lead</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info of lead</returns>
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

        /// <summary>Update information about lead</summary>
        /// <param name="leadId">Id of lead</param>
        /// /// <param name="inputModel">Nonupdated info about  lead </param>
        /// <returns>Updated info about lead</returns>
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [HttpPut("{leadId}")]
        public ActionResult<LeadOutputModel> UpdateLead(int leadId, [FromBody] UpdateLeadInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ModelState);
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

        /// <summary>Change value of parametr "IsDeleted" to 1(Deleted)</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Update lead, which is deleted</returns>
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

        /// <summary>Change value of parametr "IsDeleted" to 0(Not deleted)</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Update lead, which is not deleted</returns>
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
        /// <returns>Created city</returns>
        [ProducesResponseType(typeof(CityOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [HttpPost("city")]
        public ActionResult<CityOutputModel> AddCity([FromBody] CityInputModel city)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationException(ModelState);
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

        /// <summary>Get lead balance</summary>
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

        /// <summary>Get lead transactions</summary>
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
        
    }
}
