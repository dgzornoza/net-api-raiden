using MediatR;

namespace NetApiRaiden1.Application.Infrastructure.Queries;

public interface IQueryHandler<in TQuery, TResult> :
    IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
}
