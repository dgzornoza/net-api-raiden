using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NetApiRaiden1.Infrastructure.Domain;
using NetApiRaiden1.Test.Common.Infrastructure;

namespace NetApiRaiden1.IntegrationTest.Infrastructure;

/// <summary>
/// DbContext for testing
///
/// This context is here for to generate migrations.
///
///  - Delete Migrations folder if needed then
///  - Run following command in main, solution directory:
///
/// dotnet ef migrations add InitialMigrationSqlite --context TestDbContext -o Infrastructure/Migrations --project NetApiRaiden1.IntegrationTest
/// add-migration InitialMigrationSqlite -Context TestDbContext -o Infrastructure/Migrations -Project NetApiRaiden1.IntegrationTest
/// </summary>
public class TestDbContext : ContextSqlite<Program, TestDbContext, AppUnitOfWork>
{
    /// <summary>
    /// Empty ctor required for generating migrations from dotnet tool
    /// </summary>
    public TestDbContext() { }

    public TestDbContext(DbContextOptions<TestDbContext> options, IMediator mediator) :
        base(options, mediator)
    {
    }
}
