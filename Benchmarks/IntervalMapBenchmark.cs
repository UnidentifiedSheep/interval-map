using BenchmarkDotNet.Attributes;
using IntervalMap.Core.Models;
using IntervalMap.Variations;

namespace Benchmarks;

[MemoryDiagnoser] // измеряем память
public class IntervalMapBenchmark
{
    private IntervalMap<string, int> _map = null!;
    
    [GlobalSetup]
    public void SetupArray()
    {
        IntervalMap<string, int> _map = new IntervalMap<string, int>(10_000_000);
        GenerateIntervals();
    }

    private void GenerateIntervals()
    {
        double start = 0;
        double end = (start + Random.Shared.NextDouble() * 10000) + 0.1;
        
        for (int i = 0; i < 1000; i++)
        {
            start = end + 0.1;
            end = (start + Random.Shared.NextDouble() * 10000) + 0.1;
            _map.AddInterval(new Interval<string>(start, end));
        }
    }

    [Benchmark]
    public void GetIntervals()
    {
        var rand = Random.Shared.NextDouble() * 1000000;
        _map.GetInterval(rand);
    }
}