using NetApiRaiden1.Application.Infrastructure.Queries;

namespace NetApiRaiden1.Application.Queries.Samples.QueryableSamples;
public sealed record QueryableSamplesQuery : IQuery<IQueryable<QueryableSamplesItemDto>>;
