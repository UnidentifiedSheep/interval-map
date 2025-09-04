using IntervalMap.Core.Abstractions;
using IntervalMap.Core.Models;

namespace IntervalMap.Variations;

public class AdaptiveIntervalMap<T> : IntervalMapBase<Interval<T>> where T : class
{
    private readonly List<Interval<T>> _intervals = new();
    private readonly byte _signsAfterComma;
    private IntervalMapBase<Interval<T>> _map;

    public AdaptiveIntervalMap(byte signsAfterComma = 2)
    {
        SignsAfterComma = signsAfterComma;
        ScaleFactor = (int)Math.Pow(10, signsAfterComma);
        _signsAfterComma = signsAfterComma;
        _map = new IntervalMap<T,byte>(1, signsAfterComma);
        MaxValue = _map.MaxValue;
    }
    
    public override IntervalMapBase<Interval<T>> AddInterval(Interval<T> interval)
    {
        var scaledEnd = Scale(interval.End);
        if (scaledEnd <= _map.MaxValue)
            _map.AddInterval(interval);
        else
            IncreaseAndAddInterval(interval);
        _intervals.Add(interval);
        return this;
    }
    
    private void IncreaseAndAddInterval(Interval<T> interval)
    {
        var scaledEnd = Scale(interval.End);
        double tempMaxValue = scaledEnd > MaxValue ? interval.End : DeScale(MaxValue); 
        var newInterval = GetCorrectIntervalMap(_intervals.Count + 1, tempMaxValue);
        foreach (var inter in _intervals)
            newInterval.AddInterval(inter);
        MaxValue = newInterval.MaxValue;
        _map = newInterval;
        _map.AddInterval(interval);
    }

    private IntervalMapBase<Interval<T>> GetCorrectIntervalMap(int intervalCount, double maxValue)
    {
        return intervalCount switch
        {
            < 255  => new IntervalMap<T, byte>(maxValue, _signsAfterComma),
            < ushort.MaxValue => new IntervalMap<T, ushort>(maxValue, _signsAfterComma),
            < int.MaxValue => new IntervalMap<T, int>(maxValue, _signsAfterComma),
            _ => throw new ArgumentOutOfRangeException(nameof(intervalCount),
                $"Interval map can contain maximum {int.MaxValue}.")
        };
    }
    
    public override bool Contains(double value)
    {
        return _map.Contains(value);
    }

    public override Interval<T>? GetInterval(double value)
    {
        return _map.GetInterval(value);
    }

    public override bool RemoveInterval(Interval<T> interval)
    {
        if (!_map.RemoveInterval(interval)) return false;
        _intervals.Remove(interval);
        return true;
    }

    public override bool IsIntersectionExists(Interval<T> interval)
    {
        return _map.IsIntersectionExists(interval);
    }
}