﻿using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using $ext_safeprojectname$.Application.Queries.Samples.QueryableSamples;

namespace $safeprojectname$.Controllers.v1;

[ApiVersion(1.0)]
public class SamplesController : ApiControllerBase
{
    private readonly IMediator mediator;

    public SamplesController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    /// <summary>
    ///  GET: api/samples/{id}
    /// </summary>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QueryableSamplesItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new QueryableSamplesQuery();
        var response = await mediator.Send(query);

        var result = response.FirstOrDefault(item => item.Id == id);

        return Ok(result);
    }

    /// <summary>
    ///  GET: api/samples/
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QueryableSamplesItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        var query = new QueryableSamplesQuery();
        var response = await mediator.Send(query);

        return Ok(response.ToList());
    }
}
