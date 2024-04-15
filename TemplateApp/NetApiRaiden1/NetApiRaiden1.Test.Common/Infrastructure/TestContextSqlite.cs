using MediatR;
using Microsoft.EntityFrameworkCore;
using NetApiRaiden1.Infrastructure.Domain;

namespace NetApiRaiden1.Test.Common.Infrastructure;

/// <summary>
/// SqLite DbContext for testing
///
/// This context is here for generate migrations easy:.
///
///  - Delete Migrations folder if needed then
///  - Run following command in main, solution directory:
///
/// Inherit from this class and run anyone command:
/// - dotnet ef migrations add InitialMigrationSqlite --context {InheritedClass} -o Infrastructure/Migrations --project {IntegrationTestProject}
/// - add-migration InitialMigrationSqlite -Context {InheritedClass} -o Infrastructure/Migrations -Project {IntegrationTestProject}
/// 
/// </summary>
/// <typeparam name="TEntityConfiguration">Entity configuration type for apply configurations from assembly</typeparam>
public abstract class ContextSqlite<TProgram, TTestDbContext, TEntityConfiguration> : EfUnitOfWork<TEntityConfiguration>
    where TProgram : class
    where TTestDbContext : DbContext, IEfUnitOfWork
    where TEntityConfiguration : class
{
    /// <summary>
    /// Empty ctor required for generating migrations from dotnet tool
    /// </summary>
    protected ContextSqlite()
    {
    }

    protected ContextSqlite(DbContextOptions options, IMediator mediator) :
        base(options, mediator)
    {
    }

    /// <summary>
    /// SQLite no soporta transacciones de ambiente
    /// </summary>
    protected override bool UseTransactions => false;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(TestWebApplicationFactory<TProgram, TTestDbContext>.ConnectionString);
        optionsBuilder.ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.AmbientTransactionWarning));
    }
}
