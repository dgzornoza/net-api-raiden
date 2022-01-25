using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using $safeprojectname$.Infrastructure.Extensions;
using $safeprojectname$.Infrastructure.Filters;

namespace $safeprojectname$
{
    /// <summary>
    /// Main Startup class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="configuration">Configuration</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// App configuration
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Function for configure application container services
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">service container collection</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(configure =>
            {
                configure.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });

            services.AddAppConfiguration(this.Configuration);

            services.AddIocContainer(this.Configuration);
        }

        /// <summary>
        /// Function for configure Application request pipeline
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">application builder</param>
        /// <param name="env">Web host environment</param>
        /// <param name="provider">Provider for api version</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseStaticFiles();
            /* $identityserver_feature$ start */
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseAuthentication();
            /* $identityserver_feature$ end */

            app.UseAppConfiguration(env, provider);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
