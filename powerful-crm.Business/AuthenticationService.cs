using Microsoft.IdentityModel.Tokens;
using powerful_crm.Business.Models;
using powerful_crm.Core.Configs;
using powerful_crm.Core.Enums;
using powerful_crm.Core.Models;
using powerful_crm.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace powerful_crm.Business
{
   public class AuthenticationService : IAuthenticationService
    {
        private ILeadRepository _repo;
        public AuthenticationService(ILeadRepository repository)
        {
            _repo = repository;
        }
        public LeadDto GetAuthenticatedLead(string login)
        {
            return _repo.GetLeadCredentials(null, login);
        }
        public AuthenticationResponse GenerateToken(LeadDto lead)
        {
            var identity = GetIdentity(lead);
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new AuthenticationResponse
            {
                Token = encodedJwt
            };
        }
        private ClaimsIdentity GetIdentity(LeadDto lead)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, lead.Login),
                    new Claim(ClaimTypes.NameIdentifier, lead.Id.ToString()),
                    new Claim(ClaimsIdentity.DefaultRoleClaimType, lead.Role.ToString())
                };
            
            ClaimsIdentity claimsIdentity =
            new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }
    }
}
