namespace $safeprojectname$.SharedKernel
{
    /// <summary>
    /// Interface to declare a check of the uniqueness of values ​​that is commonly used
    /// by the entities to validate a value is unique in the whole system.
    /// </summary>
    /// <typeparam name="T">Value Type</typeparam>
    public interface IValueUniquenessChecker<T>
    {
        bool IsUnique(T value);
    }
}
