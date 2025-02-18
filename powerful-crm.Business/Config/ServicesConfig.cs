﻿using Microsoft.Extensions.DependencyInjection;
using powerful_crm.Business;
using powerful_crm.Data;

namespace EducationSystem.Business.Config
{
    public static class ServicesConfig
    {
        public static void RegistrateServicesConfig(this IServiceCollection services)
        {
            services.AddScoped<ILeadService, LeadService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ICityService, CityService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICheckerService, CheckerService>();
            services.AddScoped<IPayPalRequestService, PayPalRequestService>();

            services.AddScoped<ILeadRepository, LeadRepository>();
            services.AddScoped<ICityRepository, CityRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
        }
    }
}