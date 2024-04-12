using MediatR;

namespace NetApiRaiden1.Application.Infrastructure.Queries;

public interface IQuery<out TResult> : IRequest<TResult>
{
}
