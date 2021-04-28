using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Business;
using powerful_crm.Core;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.Enums;
using powerful_crm.Core.Models;
using powerful_crm.Core.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace powerful_crm.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController: ControllerBase
    {
        private IAccountService _accountService;
        private ILeadService _leadService;
        private Checker _checker;
        private IMapper _mapper;
        public AccountController(IOptions<AppSettings> options, IMapper mapper, IAccountService accountService, ILeadService leadService, Checker checker)
        {
            _accountService = accountService;
            _leadService = leadService;
            _checker = checker;
            _mapper = mapper;
        }
        /// <summary>Adds new account</summary>
        /// <param name="inputModel">Information about account to add</param>
        /// <returns>Information about added account</returns>
        [ProducesResponseType(typeof(AccountOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [HttpPost]
        public ActionResult<AccountOutputModel> AddAccount([FromBody] AccountInputModel inputModel)
        {
            if (!_checker.CheckIfUserIsAllowed(inputModel.LeadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);
            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            if (_leadService.GetLeadById(inputModel.LeadId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.LeadId));
            }
            var dto = _mapper.Map<AccountDto>(inputModel);
            var addedAccountId = _accountService.AddAccount(dto);
            var outputModel = _mapper.Map<AccountOutputModel>(_accountService.GetAccountById(addedAccountId));
            return Ok(outputModel);

        }
        /// <summary>Delete Account</summary>
        /// <param name="accountId">Id of lead</param>
        /// <returns>Info about account which is deleted</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("{accountId}")]
        public ActionResult DeleteAccount(int accountId)
        {
            var account = _accountService.GetAccountById(accountId);
            if (!_checker.CheckIfUserIsAllowed(account.LeadDto.Id, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);
            if (account == null)
            {
                return NotFound(string.Format(Constants.ERROR_ACCOUNT_NOT_FOUND, accountId));
            }
            if (_accountService.DeleteAccount(accountId) == 1)
                return NoContent();
            else
                return Conflict(string.Format(Constants.ERROR_ACCOUNT_HAS_DEPENDENCIES, account.Id));
        }
    }
}
