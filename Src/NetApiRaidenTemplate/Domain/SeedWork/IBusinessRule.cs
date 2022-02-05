namespace $safeprojectname$.SeedWork
{
    /// <summary>
    /// Domain business rules interface
    /// </summary>
    public interface IBusinessRule
    {
        string Message { get; }

        bool IsBroken();
    }
}
