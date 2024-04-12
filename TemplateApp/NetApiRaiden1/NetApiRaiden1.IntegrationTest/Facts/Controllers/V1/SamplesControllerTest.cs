using NetApiRaiden1.Api;
using NetApiRaiden1.Application.Queries.Samples.QueryableSamples;
using NetApiRaiden1.IntegrationTest.Infrastructure;
using NetApiRaiden1.Test.Common.Extensions;
using NetApiRaiden1.Test.Common.Infrastructure;
using NetApiRaiden1.Test.Common.Models;
using Xunit;

namespace NetApiRaiden1.IntegrationTest.Facts.Controllers.V1;

public class SamplesControllerTest : IClassFixture<TestWebApplicationFactory<Program, TestDbContext>>
{
    private readonly TestWebApplicationFactory<Program, TestDbContext> factory;

    public SamplesControllerTest(TestWebApplicationFactory<Program, TestDbContext> factory)
    {
        this.factory = factory;
    }

    /// <summary>
    /// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    [Fact]
    public async Task Get_Sample_AsExpected()
    {
        // Arrange
        var url = "api/samples";

        var query = new QueryableSamplesQuery();

        var httpRequest = new HttpRequestDataModel<QueryableSamplesQuery>(url, query);
        var client = factory.CreateAuthClient();

        // Act
        var response = await client.GetAsync<IEnumerable<QueryableSamplesItemDto>>(httpRequest);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(2, response.Count());
    }
}
