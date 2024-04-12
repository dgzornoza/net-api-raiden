namespace $safeprojectname$.SeedWork;

/// <summary>
/// Repository unit of work interface
/// </summary>
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);

    void RollbackChanges();
}
