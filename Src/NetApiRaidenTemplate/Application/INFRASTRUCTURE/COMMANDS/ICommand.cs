using MediatR;

namespace $safeprojectname$.Infrastructure.Commands;

public interface ICommand : IRequest
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{
}
