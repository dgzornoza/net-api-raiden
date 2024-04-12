using MediatR;

namespace NetApiRaiden1.Application.Infrastructure.Commands;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{
}
