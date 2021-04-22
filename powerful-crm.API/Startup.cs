using EducationSystem.Business.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using powerful_crm.API.Middleware;
using powerful_crm.Core.Configs;
using powerful_crm.Core.Settings;
using SqlKata.Compilers;
using SqlKata.Execution;
using System.Data.SqlClient;
using System.Linq;

namespace powerful_crm.API
{
    public class Startup
    {

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            _env = env;
        }

        public IConfiguration Configuration { get; }

        private IWebHostEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AuthenticationConfigExtention();
            services.AddControllers();
            services.Configure<AppSettings>(c =>
            {
                c.CONNECTION_STRING = _env.IsProduction() ?
                Configuration.GetValue<string>("CRM_CONNECTION_STRING") :
                Configuration.GetValue<string>("CRM_TEST_CONNECTION_STRING");

                c.TSTORE_URL = _env.IsProduction() ?
                Configuration.GetValue<string>("TSTORE_URL") :
                Configuration.GetValue<string>("TSTORE_TEST_URL");
            });
            services.RegistrateServicesConfig();
            services.AddAutoMapper(typeof(Startup));
            services.SwaggerExtention();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "powerful-crm");
            });
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
