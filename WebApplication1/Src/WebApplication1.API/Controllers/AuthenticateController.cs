using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using WebApplication1.Api.Infrastructure.Authorization;

namespace WebApplication1.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticateController : Controller
    {
        private readonly IIdentityServerInteractionService interaction;
        private readonly IHostEnvironment environment;

        public AuthenticateController(
            IIdentityServerInteractionService interaction,
            IWebHostEnvironment environment)
        {
            this.interaction = interaction;
            this.environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var context = await this.interaction.GetAuthorizationContextAsync(request.ReturnUrl);
            var user = IdentityConfiguration.TestUsers.FirstOrDefault(usr => usr.Password == request.Password && usr.Username == request.Username);

            if (user != null && context != null)
            {
                try
                {
                    await this.HttpContext.SignInAsync(new IdentityServerUser(user.SubjectId), new AuthenticationProperties());
                    return new JsonResult(new { RedirectUrl = request.ReturnUrl, IsOk = true });
                }
                catch (Exception ex)
                {
                    Console.Write(ex);
                    throw;
                }
            }

            return this.Unauthorized();
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var context = await this.interaction.GetLogoutContextAsync(logoutId);
            bool showSignoutPrompt = true;

            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                showSignoutPrompt = false;
            }

            if (this.User?.Identity?.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await this.HttpContext.SignOutAsync();
            }

            // no external signout supported for now (see \Quickstart\Account\AccountController.cs TriggerExternalSignout)
            return this.Ok(new
            {
                showSignoutPrompt,
                ClientName = string.IsNullOrEmpty(context?.ClientName) ? context?.ClientId : context?.ClientName,
                context?.PostLogoutRedirectUri,
                context?.SignOutIFrameUrl,
                logoutId,
            });
        }

        [HttpGet]
        [Route("Error")]
        public async Task<IActionResult> Error(string errorId)
        {
            // retrieve error details from identityserver
            var message = await this.interaction.GetErrorContextAsync(errorId);

            if (message != null)
            {
                if (!this.environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }

            return this.Ok(message);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = default!;

        public string Password { get; set; } = default!;

        public string ReturnUrl { get; set; } = default!;
    }
}
