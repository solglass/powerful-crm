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
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;
using RestSharp;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace powerful_crm.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class LeadController : ControllerBase
    {
        private IAccountService _accountService;
        private ILeadService _leadService;
        private ICityService _cityService;
        private Checker _checker;
        private IMapper _mapper;
        private RestClient _client;

        public LeadController(IOptions<AppSettings> options,
                              IMapper mapper,
                              ILeadService leadService,
                              ICityService cityService,
                              Checker checker,
                              IAccountService accountService)
        {
            _accountService = accountService;
            _leadService = leadService;
            _checker = checker;
            _cityService = cityService;
            _mapper = mapper;
            _client = new RestClient(options.Value.TSTORE_URL);
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


        /// <summary>Gets lead balance</summary>
        /// <param name="leadId">Id of lead</param>
        /// <param name="currency">presentation currency</param>
        /// <returns>Info about balance</returns>
        [ProducesResponseType(typeof(LeadBalanceOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{leadId}/balance/{currency}")]
        public async Task<ActionResult<LeadBalanceOutputModel>> GetBalanceByLeadIdAsync(int leadId, string currency)
        {
            if (!_checker.CheckIfUserIsAllowed(leadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!_checker.CheckCurrencyIsDefined(currency))
                return Conflict(Constants.ERROR_CURRENCY_NOT_SUPPORT);

            var lead = await _leadService.GetLeadByIdAsync(leadId);
            if (lead == null)
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, leadId)
                });


            var balanceModel = new BalanceMiddleModel
            {
                AccountIds = (await _accountService.GetAccountsByLeadIdAsync(leadId)).ConvertAll(acc => acc.Id),
                Currency = currency
            };
            var queryResult = await RequestToTransactionStoreAsync(balanceModel, Constants.API_GET_BALANCE);
            return Ok(queryResult);
        }

        /// <summary>Gets lead transactions</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about transactions</returns>
        [ProducesResponseType(typeof(List<TransactionOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("{leadId}/transactions")]
        public async Task<ActionResult<List<TransactionOutputModel>>> GetTransactionsByLeadIdAsync(int leadId)
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

            var request = new RestRequest(string.Format(Constants.API_GET_TRANSACTION, leadId), Method.GET);
            var queryResult = (await _client.ExecuteAsync<string>(request)).Data;

            return Ok(queryResult);
        }

        /// <summary>Adds deposit</summary>
        /// <param name="inputModel">Information about deposit</param>
        /// <returns>Id of added deposit</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("deposit")]
        public async Task<ActionResult<int>> AddDepositAsync([FromBody] TransactionInputModel inputModel)
        {
            if (!_checker.CheckIfUserIsAllowed(inputModel.LeadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);

            if (await _leadService.GetLeadByIdAsync(inputModel.LeadId) == null)
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.LeadId)
                });

            var transactionModel = _mapper.Map<TransactionMiddleModel>(inputModel);
            var queryResult = await RequestToTransactionStoreAsync(transactionModel, Constants.API_DEPOSIT);
            return Ok(queryResult);
        }

        /// <summary>Adds withdraw</summary>
        /// <param name="inputModel">Information about withdraw</param>
        /// <returns>Id of added withdraw</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("withdraw")]
        public async Task<ActionResult<int>> AddWithdrawAsync([FromBody] TransactionInputModel inputModel)
        {
            if (!_checker.CheckIfUserIsAllowed(inputModel.LeadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);

            if (await _leadService.GetLeadByIdAsync(inputModel.LeadId) == null)
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.LeadId)
                });
            
            var transactionModel = _mapper.Map<TransactionMiddleModel>(inputModel);
            var queryResult = await RequestToTransactionStoreAsync(transactionModel, Constants.API_WITHDRAW);
            return Ok(queryResult);
        }

        /// <summary>Adds transfer</summary>
        /// <param name="inputModel">Information about transfer</param>
        /// <returns>Id of added transfer</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("transfer")]
        public async Task<ActionResult<int>> AddTransferAsync([FromBody] TransferInputModel inputModel)
        {
            if (!_checker.CheckIfUserIsAllowed(inputModel.SenderId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!ModelState.IsValid)
                throw new CustomValidationException(ModelState);

            if (await _leadService.GetLeadByIdAsync(inputModel.RecipientId) == null)
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.RecipientId)
                });

            if (await _leadService.GetLeadByIdAsync(inputModel.SenderId) == null)
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.SenderId)
                });

            var transferModel = _mapper.Map<TransferMiddleModel>(inputModel);
            var queryResult = await RequestToTransactionStoreAsync(transferModel, Constants.API_TRANSFER);
            return Ok(queryResult);
        }
       
        private async Task<int> RequestToTransactionStoreAsync<T>(T middleModel, string apiUrl)
        {
            var request = new RestRequest(apiUrl, Method.POST);
            request.AddParameter("application/json", JsonSerializer.Serialize(middleModel), ParameterType.RequestBody);
            return (await _client.ExecuteAsync<int>(request)).Data;
        }
    }
}
