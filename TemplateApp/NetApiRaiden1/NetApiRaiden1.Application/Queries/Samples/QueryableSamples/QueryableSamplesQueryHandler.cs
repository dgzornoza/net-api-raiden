using Microsoft.AspNetCore.Http;
using NetApiRaiden1.Application.Infrastructure.Queries;

namespace NetApiRaiden1.Application.Queries.Samples.QueryableSamples;

public class QueryableEventsQueryHandler : IQueryHandler<QueryableSamplesQuery, IQueryable<QueryableSamplesItemDto>>
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public QueryableEventsQueryHandler(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<IQueryable<QueryableSamplesItemDto>> Handle(QueryableSamplesQuery request, CancellationToken cancellationToken)
    {
        // generate samples
        var samples = new List<QueryableSamplesItemDto>
        {
            new() {
                Id = Guid.NewGuid(),
                Name = "Sample 1",
                Description = "Description 1"
            },
            new() {
                Id = Guid.NewGuid(),
                Name = "Sample 2",
                Description = "Description 2"
            }
        }.AsQueryable();

        return await Task.FromResult(samples);
    }
}
