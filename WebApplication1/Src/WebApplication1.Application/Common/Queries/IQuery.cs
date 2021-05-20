using MediatR;

namespace WebApplication1.Application.Common.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
