using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace powerful_crm.API
{
    public static class SwaggerConfig
    {
        public static void SwaggerExtention(this IServiceCollection services)
        {
            services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "powerful-crm" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                swagger.IncludeXmlComments(xmlPath);
                swagger.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            services.AddSwaggerGenNewtonsoftSupport();
        }
    }
}
