namespace IntervalMap.Core.Interfaces;

public interface IPage<TMap>
{
    TMap? Get(int pos); 
    void Set(int pos, TMap? value);
}