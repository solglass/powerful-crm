using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using powerful_crm.API.Models.InputModels;
using powerful_crm.API.Models.OutputModels;
using powerful_crm.Business;
using powerful_crm.Core;
using powerful_crm.Core.CustomExceptions;
using powerful_crm.Core.Models;
using System.Threading.Tasks;

namespace powerful_crm.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;
        private ILeadService _leadService;
        private Checker _checker;
        private IMapper _mapper;
        public AccountController(IMapper mapper, IAccountService accountService, ILeadService leadService, Checker checker)
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
        public async Task<ActionResult<AccountOutputModel>> AddAccountAsync([FromBody] AccountInputModel inputModel)
        {
            if (!_checker.CheckIfUserIsAllowed(inputModel.LeadId, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);
            if (!ModelState.IsValid)
            {
                throw new CustomValidationException(ModelState);
            }
            if (await _leadService.GetLeadByIdAsync(inputModel.LeadId) == null)
            {
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_ID, inputModel.LeadId));
            }
            var dto = _mapper.Map<AccountDto>(inputModel);
            var addedAccountId = await _accountService.AddAccountAsync(dto);
            var outputModel = _mapper.Map<AccountOutputModel>(await _accountService.GetAccountByIdAsync(addedAccountId));
            return Ok(outputModel);

        }
        /// <summary>Delete Account</summary>
        /// <param name="accountId">Id of lead</param>
        /// <returns>Info about account which is deleted</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [HttpDelete("{accountId}")]
        public async Task<ActionResult> DeleteAccount(int accountId)
        {
            var account = await _accountService.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return NotFound(string.Format(Constants.ERROR_ACCOUNT_NOT_FOUND, accountId));
            }
            if (!_checker.CheckIfUserIsAllowed(account.LeadDto.Id, HttpContext))
                throw new ForbidException(Constants.ERROR_NOT_ALLOWED_ACTIONS_WITH_OTHER_LEAD);
            await _accountService.DeleteAccountAsync(accountId);
            return NoContent();

        }
    }
}
