namespace NetApiRaiden1.Domain.SeedWork;

/// <summary>
/// Domain business rules interface
/// </summary>
public interface IBusinessRule
{
    string Message { get; }

    bool IsBroken();
}
