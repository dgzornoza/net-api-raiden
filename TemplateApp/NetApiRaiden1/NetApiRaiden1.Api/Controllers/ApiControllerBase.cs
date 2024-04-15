using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Attributes;

namespace NetApiRaiden1.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ODataIgnored]
public class ApiControllerBase : ControllerBase
{
}
