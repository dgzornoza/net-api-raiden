using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApplication1.Api;
using WebApplication1.Api.Infrastructure.Extensions;
using WebApplication1.Api.Infrastructure.Filters;

namespace WebApplication1.IntegrationTest
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(configure =>
            {
                configure.Filters.Add(typeof(HttpGlobalExceptionFilter));
            })
            .AddApplicationPart(Assembly.GetAssembly(typeof(Startup)));

            services.AddAppConfiguration(this.Configuration);

            services.AddIocContainer(this.Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            app.UseRouting();

            app.UseAppConfiguration(env, provider);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
