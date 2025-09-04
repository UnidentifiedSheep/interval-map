using IntervalMap.Core.Interfaces;

namespace IntervalMap.Pages;

public class SparsePage<TMap> : IPage<TMap>
{
    private readonly Dictionary<int, TMap?> _data = new();
    public TMap? Get(int pos) => _data.GetValueOrDefault(pos);
    public void Set(int pos, TMap? value) => _data[pos] = value;
}