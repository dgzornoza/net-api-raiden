using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace NetApiRaiden1.Application.Infrastructure.Commands;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
{
    protected readonly ILogger logger;
    protected readonly IMediator mediator;
    protected readonly IHttpContextAccessor httpContextAccessor;

    public CommandHandler(ILogger logger, IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        this.logger = logger;
        this.mediator = mediator;
        this.httpContextAccessor = httpContextAccessor;
    }

    public abstract Task Handle(TCommand request, CancellationToken cancellationToken);
}

public abstract class CommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult>
{
    protected readonly ILogger logger;
    protected readonly IMediator mediator;
    protected readonly IHttpContextAccessor httpContextAccessor;

    public CommandHandler(ILogger logger, IMediator mediator, IHttpContextAccessor httpContextAccessor)
    {
        this.logger = logger;
        this.mediator = mediator;
        this.httpContextAccessor = httpContextAccessor;
    }

    public abstract Task<TResult> Handle(TCommand request, CancellationToken cancellationToken);
}
