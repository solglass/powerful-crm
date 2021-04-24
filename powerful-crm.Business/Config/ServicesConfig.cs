using Microsoft.Extensions.DependencyInjection;
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
            services.AddScoped<IAccountService, AccountService>();

            services.AddScoped<ILeadRepository, LeadRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();

        }
    }
}