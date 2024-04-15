using MediatR;
using Microsoft.EntityFrameworkCore;

namespace $safeprojectname$.Domain;


/// <summary>
/// App Entity Framework unit of work
/// </summary>
public class AppUnitOfWork : EfUnitOfWork<AppUnitOfWork>
{    
    /// <summary>
    /// Empty ctor required for generating migrations from dotnet tool
    /// </summary>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    internal AppUnitOfWork()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
    }

    public AppUnitOfWork(DbContextOptions<AppUnitOfWork> options, IMediator mediator)
        : base(options, mediator)
    {
    }

    protected override bool UseTransactions => true;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customize SSO DB collation
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");
    }
}
