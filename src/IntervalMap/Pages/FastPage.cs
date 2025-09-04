using IntervalMap.Core.Interfaces;

namespace IntervalMap.Pages;

public class FastPage<TMap>(int size) : IPage<TMap>
{
    private readonly TMap?[] _data = new TMap?[size];
    public TMap? Get(int pos) => _data[pos];
    public void Set(int pos, TMap? value) => _data[pos] = value;
}