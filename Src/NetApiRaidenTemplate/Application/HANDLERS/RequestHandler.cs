using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace $safeprojectname$.Handlers;

public abstract class RequestHandler
{
    protected RequestHandler(ILogger logger, IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        Logger = logger;
        Mediator = mediator;
        ContextAccessor = httpContextAccessor;
    }

    protected ILogger Logger { get; }
    protected IHttpContextAccessor ContextAccessor { get; }
    protected IMediator Mediator { get; }
}
