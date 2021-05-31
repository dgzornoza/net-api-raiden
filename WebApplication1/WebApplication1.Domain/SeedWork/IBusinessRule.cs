namespace WebApplication1.Domain.SeedWork
{
    public interface IBusinessRule
    {
        string Message { get; }

        bool IsBroken();
    }
}
