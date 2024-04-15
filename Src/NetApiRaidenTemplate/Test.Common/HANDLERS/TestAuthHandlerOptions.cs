using Microsoft.AspNetCore.Authentication;

namespace $safeprojectname$.Handlers;

public class TestAuthHandlerOptions : AuthenticationSchemeOptions
{
    public string DefaultUserName { get; set; } = default!;
    public string DefaultUserRole { get; set; } = default!;
}
