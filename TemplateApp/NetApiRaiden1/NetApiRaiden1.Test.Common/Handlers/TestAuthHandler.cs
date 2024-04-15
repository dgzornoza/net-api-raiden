using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace NetApiRaiden1.Test.Common.Handlers;

public class TestAuthHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    public const string UserName = "UserName";
    public const string UserRole = "UserRole";
    public const string AuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;

    private readonly string defaultUserName;
    private readonly string defaultUserRole;

    public TestAuthHandler(
        IOptionsMonitor<TestAuthHandlerOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
        defaultUserName = options.CurrentValue.DefaultUserName;
        defaultUserRole = options.CurrentValue.DefaultUserRole;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = ExtractClaimsFromRequestHeaders() ?? new List<Claim>();

        var identity = new ClaimsIdentity(claims, AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

        var result = AuthenticateResult.Success(ticket);

        return Task.FromResult(result);
    }

    private IEnumerable<Claim> ExtractClaimsFromRequestHeaders()
    {
        var claims = new List<Claim>();

        // Nombre
        if (Context.Request.Headers.TryGetValue(UserName, out var userName))
        {
            claims.Add(new Claim("name", userName[0] ?? ""));
        }
        else
        {
            claims.Add(new Claim("name", defaultUserName));
        }

        // Rol
        if (Context.Request.Headers.TryGetValue(UserRole, out var userRole))
        {
            claims.Add(new Claim("role", userRole[0] ?? ""));
        }
        else
        {
            claims.Add(new Claim("role", defaultUserRole));
        }

        return claims;
    }
}
