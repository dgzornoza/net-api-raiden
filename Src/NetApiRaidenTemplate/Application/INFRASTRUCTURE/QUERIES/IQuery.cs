using MediatR;

namespace $safeprojectname$.Infrastructure.Queries;

public interface IQuery<out TResult> : IRequest<TResult>
{
}
