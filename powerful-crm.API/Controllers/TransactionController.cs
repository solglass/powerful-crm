using AutoMapper;
using Google.Authenticator;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.MiddleModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Business;
using powerful_crm.Core;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.Settings;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

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
        private MemoryCacheSingleton _validatedModelCache;
        private MemoryCacheTimer _timer;
        private int _timerInterval = 60000;
        private long _cashCleaningInterval = 3000000000;

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
            _validatedModelCache = MemoryCacheSingleton.GetCacheInstance();
            _timer = new MemoryCacheTimer(_timerInterval);
        }

        /// <summary>Gets lead balance</summary>
        /// <param name="leadId">Id of lead</param>
        /// <param name="currency">presentation currency</param>
        /// <returns>Info about balance</returns>
        [ProducesResponseType(typeof(LeadBalanceOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("lead/{leadId}/balance/{currency}")]
        public async Task<ActionResult<LeadBalanceOutputModel>> GetBalanceByLeadIdAsync(int leadId, string currency)
        {
            if (!_checker.CheckIfUserIsAllowed(leadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            if (!_checker.CheckCurrencyIsDefined(currency))
                return Conflict(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status409Conflict,
                    Message = Constants.ERROR_CURRENCY_NOT_SUPPORT
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
            var queryResult = await GetResponseAsync<LeadBalanceOutputModel>(Constants.API_GET_BALANCE, Method.POST, balanceModel);
            return Ok(queryResult);
        }

        /// <summary>Gets lead transactions</summary>
        /// <param name="leadId">Id of lead</param>
        /// <returns>Info about transactions</returns>
        [ProducesResponseType(typeof(List<TransactionOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpGet("lead/{leadId}")]
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

            var queryResult = (await GetResponseAsync<string>(Constants.API_GET_TRANSACTION, Method.POST, lead.Accounts.ConvertAll((acc) => acc.Id))).Data;

            return Ok(queryResult);
        }

        /// <summary>Adds deposit</summary>
        /// <param name="leadId"></param>
        /// <param name="inputModel">Information about deposit</param>
        /// <returns>Id of added deposit</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("lead/{leadId}/deposit")]
        public async Task<ActionResult<int>> AddDepositAsync(int leadId, [FromBody] TransactionInputModel inputModel)
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

            var account = lead.Accounts.Find((acc) => acc.Id == inputModel.AccountId);
            if (account == null)
            {
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_ACCOUNT_NOT_FOUND, inputModel.AccountId)
                });
            }

            var transactionModel = _mapper.Map<TransactionMiddleModel>(inputModel);
            transactionModel.Account.Currency = account.Currency.ToString();
            var queryResult = (await GetResponseAsync<int>(Constants.API_DEPOSIT, Method.POST, transactionModel)).Data;
            return Ok(queryResult);
        }

        /// <summary>Adds withdraw</summary>
        /// <param name="leadId"></param>
        /// <param name="inputModel">Information about withdraw</param>
        /// <returns>Id of added withdraw</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("{leadId}/withdraw")]
        public async Task<ActionResult<int>> AddWithdrawAsync(int leadId, [FromBody] TransactionInputModel inputModel)
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

            var account = lead.Accounts.Find((acc) => acc.Id == inputModel.AccountId);
            if (account == null)
            {
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_ACCOUNT_NOT_FOUND, inputModel.AccountId)
                });
            }

            var memoryCacheKey = (long)DateTime.Now.Ticks;
            _validatedModelCache.Cache.Set<TransactionInputModel>(memoryCacheKey, inputModel);

            _timer.SubscribeToTimer((Object source, ElapsedEventArgs e) => DeleteModelFromCache(memoryCacheKey));

            return Ok(memoryCacheKey);
        }

        private async void DeleteModelFromCache(long memoryCacheKey)
        {
            if ((long)DateTime.Now.Ticks - memoryCacheKey > _cashCleaningInterval)
            {
                _validatedModelCache.Cache.Remove(memoryCacheKey);
                _timer.UnsubscribeFromTimer((Object source, ElapsedEventArgs e) => DeleteModelFromCache(memoryCacheKey));
            }
            Console.WriteLine("Все удалил");
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
        [HttpPost("{leadId}/providewithdraw")]
        public async Task<ActionResult<int>> ProvideWithdraw(int leadId, int memoryCacheKey, string inputCode)
        {

            if (!_validatedModelCache.Cache.TryGetValue(memoryCacheKey, out TransactionInputModel inputModel))
            {
                return NotFound("operation not found");
            }

            if (!_checker.CheckIfUserIsAllowed(leadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);

            var lead = await _leadService.GetLeadByIdAsync(leadId);

            TwoFactorAuthenticator twoFactor = new TwoFactorAuthenticator();
            bool isValid = twoFactor.ValidateTwoFactorPIN(TwoFactor.TwoFactorKey(lead.Email), inputCode);
            if (!isValid)
                throw new ForbidException("Operation not confirmed");

            var middle = _mapper.Map<TransactionMiddleModel>(inputModel);
            var request = new RestRequest(Constants.API_WITHDRAW, Method.POST);
            request.AddParameter("application/json", JsonSerializer.Serialize(middle), ParameterType.RequestBody);
            var queryResult = _client.Execute<int>(request).Data;

            _validatedModelCache.Cache.Remove(memoryCacheKey);

            return Ok(queryResult);
        }

        /// <summary>Adds transfer</summary>
        /// <param name="leadId"></param>
        /// <param name="inputModel">Information about transfer</param>
        /// <returns>Id of added transfer</returns>
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CustomExceptionOutputModel), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpPost("lead/{leadId}/transfer")]
        public async Task<ActionResult<int>> AddTransferAsync(int leadId, [FromBody] TransferInputModel inputModel)
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

            var accountSender = lead.Accounts.Find((acc) => acc.Id == inputModel.SenderAccountId);
            var accountRecepient = await _accountService.GetAccountByIdAsync(inputModel.RecipientAccountId);
            if (accountSender == null || accountRecepient == null)
            {
                return NotFound(new CustomExceptionOutputModel
                {
                    Code = StatusCodes.Status404NotFound,
                    Message = string.Format(Constants.ERROR_ACCOUNT_NOT_FOUND, accountSender == null ? inputModel.SenderAccountId : inputModel.RecipientAccountId)
                });
            }

            var transferModel = new TransferMiddleModel
            {
                SenderAccount = _mapper.Map<AccountMiddleModel>(accountSender),
                RecipientAccount = _mapper.Map<AccountMiddleModel>(accountRecepient),
                Amount = inputModel.Amount
            };

            var queryResult = (await GetResponseAsync<int>(Constants.API_TRANSFER, Method.POST, transferModel)).Data;
            return Ok(queryResult);
        }

        private async Task<IRestResponse<T>> GetResponseAsync<T>(string url, Method method, object requestBodyObject = null, string token = null)
        {
            var request = new RestRequest(url, method);

            if (token != null)
                request.AddHeader("Authorization", $"Bearer {token}");

            if (requestBodyObject != null)
                request.AddParameter("application/json", JsonSerializer.Serialize(requestBodyObject), ParameterType.RequestBody);

            return await _client.ExecuteAsync<T>(request);
        }
    }
}
