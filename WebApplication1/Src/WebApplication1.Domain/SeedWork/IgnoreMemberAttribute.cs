using System;

namespace WebApplication1.Domain.SeedWork
{
    /// <summary>
    /// Attribute to specify member should be ignored
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class IgnoreMemberAttribute : Attribute
    {
    }
}
