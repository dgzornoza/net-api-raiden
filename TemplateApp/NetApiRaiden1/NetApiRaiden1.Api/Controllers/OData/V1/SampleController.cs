using Asp.Versioning;
using Asp.Versioning.OData;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using NetApiRaiden1.Application.Queries.Samples.QueryableSamples;

namespace NetApiRaiden1.Api.Controllers.OData.V1;

[ApiVersion(1.0)]
public class SampleController : ODataController
{
    private readonly IMediator mediator;

    public SampleController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    /// OData GET odata/events
    /// </summary>
    /// <returns></returns>
    [EnableQuery]
    [HttpGet]
    [ProducesResponseType(typeof(ODataValue<IEnumerable<QueryableSamplesItemDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(ODataQueryOptions<QueryableSamplesItemDto> options)
    {
        var query = new QueryableSamplesQuery();
        var response = await mediator.Send(query);

        return Ok(response);
    }
}
