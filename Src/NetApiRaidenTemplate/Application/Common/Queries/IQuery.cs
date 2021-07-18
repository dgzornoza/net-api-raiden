using MediatR;

namespace $safeprojectname$.Common.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
