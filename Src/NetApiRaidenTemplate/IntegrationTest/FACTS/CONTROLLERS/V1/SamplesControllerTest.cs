using $ext_safeprojectname$.Api;
using $ext_safeprojectname$.Application.Queries.Samples.QueryableSamples;
using $safeprojectname$.Infrastructure;
using $ext_safeprojectname$.Test.Common.Extensions;
using $ext_safeprojectname$.Test.Common.Infrastructure;
using $ext_safeprojectname$.Test.Common.Models;
using Xunit;

namespace $safeprojectname$.Facts.Controllers.V1;

public class SamplesControllerTest : IClassFixture<TestWebApplicationFactory<Program, TestDbContext>>
{
    private readonly TestWebApplicationFactory<Program, TestDbContext> factory;

    public SamplesControllerTest(TestWebApplicationFactory<Program, TestDbContext> factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task Get_Samples_AsExpected()
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
