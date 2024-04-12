using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace NetApiRaiden1.Application.Services.Audit;

public class AuditBufferService : IAuditBufferService
{
    private static readonly ConcurrentBag<AuditDto> Audits = new();

    private static readonly object LockObj = new();

    public AuditBufferService()
    {
    }

    public void Add(AuditDto entry) => Audits.Add(entry);

    public bool TryTake([MaybeNullWhen(false)] out AuditDto entry) => Audits.TryTake(out entry);

    public bool TryPeek([MaybeNullWhen(false)] out AuditDto entry) => Audits.TryPeek(out entry);

    public IEnumerable<AuditDto> TakeAll()
    {
        lock (LockObj)
        {
            var result = new List<AuditDto>();

            while (!Audits.IsEmpty)
            {
                if (Audits.TryTake(out var entry))
                {
                    result.Add(entry);
                }
            }

            return result;
        }
    }
}
