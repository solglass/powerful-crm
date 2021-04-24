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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Controllers
{
    public class AccountController: ControllerBase
    {
        private IAccountService _accountService;
        private IMapper _mapper;
        public AccountController(IOptions<AppSettings> options, IMapper mapper, IAccountService accountService)
        {
            _accountService = accountService;
            _mapper = mapper;
        }
        /// <summary>Adds new account</summary>
        /// <param name="inputModel">Information about account to add</param>
        /// <returns>Information about added account</returns>
        [ProducesResponseType(typeof(AccountOutputModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [HttpPost]
        public ActionResult<AccountOutputModel> AddAccount([FromBody] AccountInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            var dto = _mapper.Map<AccountDto>(inputModel);
            var addedAccountId = _accountService.AddAccount(dto);
            var outputModel = _mapper.Map<AccountOutputModel>(_accountService.GetAccountById(addedAccountId));
            return Ok(outputModel);

        }
        /// <summary>Delete Account</summary>
        /// <param name="accountId">Id of lead</param>
        /// <returns>Info about account which is deleted</returns>
        [ProducesResponseType(typeof(List<LeadOutputModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [HttpDelete("{accountId}")]
        public ActionResult<LeadOutputModel> DeleteAccount(int accountId)
        {
            var account = _accountService.GetAccountById(accountId);
            if (account == null)
            {
                return NotFound(string.Format(Constants.ERROR_ACCOUNTNOTFOUND, accountId));
            }
            _accountService.DeleteAccount(accountId);
            var dto = _mapper.Map<AccountOutputModel>(_accountService.GetAccountById(accountId));
            return Ok(dto);
        }
    }
}
