using System.Diagnostics;

namespace NetApiRaiden1.Test.Common.Helpers;

public static class BenchmarkHelpers
{
    public static long Benchmark(Action action, int interval)
    {
#pragma warning disable S1215 // "GC.Collect" should not be called
        GC.Collect();
#pragma warning restore S1215 // "GC.Collect" should not be called

        var sw = Stopwatch.StartNew();

        for (var i = 0; i < interval; i++)
        {
            action.Invoke();
        }
        sw.Stop();

        return sw.ElapsedMilliseconds;
    }
}
