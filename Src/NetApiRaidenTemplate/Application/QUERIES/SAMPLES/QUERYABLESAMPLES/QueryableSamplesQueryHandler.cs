using Microsoft.AspNetCore.Http;
using $safeprojectname$.Infrastructure.Queries;

namespace $safeprojectname$.Queries.Samples.QueryableSamples;

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
                Id = Guid.Parse("CAE3D281-8D92-4BFB-A2AC-A9B2AF0BB49D"),
                Name = "Sample 1",
                Description = "Description 1"
            },
            new() {
                Id = Guid.Parse("BF44E99E-B926-4ECE-AF12-F7B3180387ED"),
                Name = "Sample 2",
                Description = "Description 2"
            }
        }.AsQueryable();

        return await Task.FromResult(samples);
    }
}
