using Microsoft.AspNetCore.Authentication;

namespace NetApiRaiden1.Test.Common.Handlers;

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public string DefaultUserName { get; set; } = default!;
    public string DefaultUserRole { get; set; } = default!;
}
