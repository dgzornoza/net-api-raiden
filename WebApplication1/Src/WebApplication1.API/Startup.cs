/* $identityserver_feature$ start */
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IdentityServer4.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
/* $identityserver_feature$ end */
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
/* $identityserver_feature$ start */
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Api.Infrastructure;
/* $identityserver_feature$ end */
using WebApplication1.Api.Infrastructure.Extensions;
using WebApplication1.Api.Infrastructure.Filters;

namespace WebApplication1.Api
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

            /* $identityserver_feature$ start */
            services.AddIdentityServer(options =>
            {
                options.UserInteraction = new UserInteractionOptions()
                {
                    LogoutUrl = "set logout url",
                    LoginUrl = "set login url",
                    LoginReturnUrlParameter = "returnUrl",
                };
            })
            .AddInMemoryClients(IdentityConfiguration.Clients)
            .AddInMemoryIdentityResources(IdentityConfiguration.IdentityResources)
            .AddInMemoryApiResources(IdentityConfiguration.ApiResources)
            .AddInMemoryApiScopes(IdentityConfiguration.ApiScopes)
            .AddTestUsers(IdentityConfiguration.TestUsers)
            .AddDeveloperSigningCredential();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = this.Configuration["JWT:ValidAudience"],
                    ValidIssuer = this.Configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.Configuration["JWT:Secret"])),
                };

                ////options.TokenValidationParameters.ValidIssuers = new[] { "https://identity.important.stuff", "https://identity.newaddress.important.stuff" };
                ////options.SecurityTokenValidators.Clear();
                ////options.SecurityTokenValidators.Add(new JwtSecurityTokenHandler
                ////{
                ////    MapInboundClaims = false,
                ////});
                ////options.TokenValidationParameters.NameClaimType = "name";
                ////options.TokenValidationParameters.RoleClaimType = "role";
            });
            /* $identityserver_feature$ end */
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
