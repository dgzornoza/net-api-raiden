
using $ext_safeprojectname$.Domain.SharedKernel.Rules;

namespace $safeprojectname$.Mocks;

public class MockValueChecker<T> : IValueUniquenessChecker<T>
{
    private readonly Func<T, bool> checkFunction;

    public MockValueChecker(Func<T, bool> checkFunction) => this.checkFunction = checkFunction;

    public bool IsUnique(T value)
    {
        return checkFunction(value);
    }
}
