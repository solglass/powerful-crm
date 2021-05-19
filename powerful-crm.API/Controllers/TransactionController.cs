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
using powerful_crm.Core.Settings;
using RestSharp;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace powerful_crm.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private RestClient _client;
        private Checker _checker;
        private IMapper _mapper;
        private IAccountService _accountService;
        private ILeadService _leadService;

        public TransactionController(IOptions<AppSettings> options,
                              IMapper mapper,
                              ILeadService leadService,
                              ICityService cityService,
                              Checker checker,
                              IAccountService accountService)
        {
            _accountService = accountService;
            _leadService = leadService;
            _checker = checker;
            _mapper = mapper;
            _client = new RestClient(options.Value.TSTORE_URL);
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
                return Conflict( new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status409Conflict,
                   Message= Constants.ERROR_CURRENCY_NOT_SUPPORT
                });

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
