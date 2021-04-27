using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using powerful_crm.API.Models.InputModels;
using powerful_crm.Business;
using powerful_crm.Business.Models;
using powerful_crm.Core;
using powerful_crm.Core.CustomExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace powerful_crm.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private IAuthenticationService _authService;
        private ISecurityService _securityService;

        public AuthenticationController(IAuthenticationService authService, ISecurityService securityService)
        {
            _securityService = securityService;
            _authService = authService;
        }
        /// <summary>Log in</summary>
        /// <param name="model">Lead credentials</param>
        /// <returns>Token</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        public ActionResult<AuthenticationResponse> Authentificate([FromBody] LoginInputModel model)
        {    
            var lead = _authService.GetAuthenticatedLead(model.Login);
            if (lead == null)
                return NotFound(string.Format(Constants.ERROR_LEAD_NOT_FOUND_BY_LOGIN, model.Login));

            if (!_securityService.VerifyPassword(lead.Password, model.Password))
                throw new WrongCredentialsException();

            var token = _authService.GenerateToken(lead);
            return Ok(token);
        }
    }
}
