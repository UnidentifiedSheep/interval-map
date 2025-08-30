using System.Numerics;
using Core.Abstractions;
using Core.Enums;
using Core.Models;

namespace IntervalMap.Variations;

public class IntervalMap<TValue, TMap> : IntervalMapBase<Interval<TValue>>
    where TValue : class
    where TMap : struct, INumber<TMap>
{
    private readonly TMap[]?[] _pages;
    private readonly int _pageCount;

    private readonly Dictionary<TMap, Interval<TValue>> _intervals = new();
    private readonly Dictionary<Interval<TValue>, TMap> _intervalToIndex = new();

    private TMap[]? _lastPage;
    private int _lastPageIndex = -1;

    private readonly TMap _emptyValue = TMap.Zero;
    private readonly TMap _one = TMap.One;

    public IntervalMap(double maxValue, byte signsAfterComma = 2, int pageSize = 1000)
    {
        SignsAfterComma = signsAfterComma;
        ScaleFactor = (int)Math.Pow(10, signsAfterComma);
        PageSize = pageSize;
        MaxValue = Scale(maxValue)+1;

        _pageCount = (int)Math.Ceiling((MaxValue + 1) / (double)PageSize);
        _pages = new TMap[_pageCount][];
    }

    public override IntervalMapBase<Interval<TValue>> AddInterval(Interval<TValue> interval)
    {
        int scaledStart = Scale(interval.Start);
        if (interval.StartBoundary == IntervalBoundary.Exclusive)
            scaledStart += 1;

        int scaledEnd = Scale(interval.End);
        if (interval.EndBoundary == IntervalBoundary.Inclusive)
            scaledEnd += 1;

        if (scaledEnd > MaxValue)
            throw new ArgumentOutOfRangeException(nameof(interval), "Interval leaves ranges of map");

        TMap intervalIndex;
        try
        {
            intervalIndex = TMap.CreateChecked(_intervals.Count);
        }
        catch (OverflowException)
        {
            throw new InvalidOperationException(
                $"Cannot add interval: current intervals count {_intervals.Count} cannot be represented by {typeof(TMap).Name}.");
        }

        if (IsIntersectionExists(interval))
            throw new ArgumentException("Interval cannot intersect other intervals");

        _intervals[intervalIndex] = interval;
        _intervalToIndex[interval] = intervalIndex;
        
        TMap storedIndex = intervalIndex + _one;

        ProcessPages(scaledStart, scaledEnd, (pageIndex, startPos, endPos) =>
        {
            FillPage(pageIndex, startPos, endPos, storedIndex);
        });

        return this;
    }

    public override bool RemoveInterval(Interval<TValue> interval)
    {
        if (!_intervalToIndex.TryGetValue(interval, out var intervalIndex))
            return false;

        var scaledStart = Scale(interval.Start);
        var scaledEnd = Scale(interval.End);

        TMap storedIndex = intervalIndex + _one;

        ProcessPages(scaledStart, scaledEnd, (pageIndex, startPos, endPos) =>
        {
            ClearPage(pageIndex, startPos, endPos, storedIndex);
        });

        _intervals.Remove(intervalIndex);
        _intervalToIndex.Remove(interval);
        return true;
    }

    public override bool Contains(double value)
    {
        int scaled = Scale(value);
        int pageIdx = GetPageIndex(scaled);
        int pos = GetPositionInPage(scaled);

        return !GetPage(pageIdx)[pos].Equals(_emptyValue);
    }

    public override Interval<TValue>? GetInterval(double value)
    {
        int scaled = Scale(value);
        int pageIdx = GetPageIndex(scaled);
        int pos = GetPositionInPage(scaled);

        var stored = GetPage(pageIdx)[pos];
        if (stored.Equals(_emptyValue))
            return null;
        
        var realIndex = stored - _one;
        return _intervals[realIndex];
    }

    public override bool IsIntersectionExists(Interval<TValue> interval)
    {
        var scaledStart = Scale(interval.Start);
        var scaledEnd = Scale(interval.End);

        bool exists = false;
        ProcessPages(scaledStart, scaledEnd, (pageIdx, startPos, endPos) =>
        {
            if (exists) return;
            var page = GetPage(pageIdx);

            for (int i = startPos; i < endPos; i++)
            {
                if (page[i].Equals(_emptyValue)) continue;
                exists = true;
                break;
            }
        });

        return exists;
    }

    private TMap[] GetPage(int pageIndex)
    {
        if (pageIndex >= _pageCount)
            throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index out of range");

        if (pageIndex == _lastPageIndex)
            return _lastPage!;

        var page = _pages[pageIndex] ?? CreatePage(pageIndex);

        _lastPage = page;
        _lastPageIndex = pageIndex;
        return page;
    }

    private void FillPage(int pageIndex, int startPos, int endPos, TMap storedIndex)
    {
        var page = _pages[pageIndex] ?? CreatePage(pageIndex);
        for (int i = startPos; i < endPos; i++)
            page[i] = storedIndex;
    }

    private void ClearPage(int pageIndex, int startPos, int endPos, TMap storedIndex)
    {
        var page = _pages[pageIndex] ?? CreatePage(pageIndex);
        for (int i = startPos; i < endPos; i++)
            if (page[i].Equals(storedIndex))
                page[i] = _emptyValue;
    }

    private void ProcessPages(int scaledStart, int scaledEnd, Action<int, int, int> action)
    {
        int startPage = GetPageIndex(scaledStart);
        int endPage = GetPageIndex(scaledEnd);

        int startPos = GetPositionInPage(scaledStart);
        int endPos = GetPositionInPage(scaledEnd);

        if (startPage == endPage)
            action(startPage, startPos, endPos);
        else
        {
            action(startPage, startPos, PageSize);

            for (int p = startPage + 1; p < endPage; p++)
                action(p, 0, PageSize);

            action(endPage, 0, endPos);
        }
    }

    private TMap[] CreatePage(int pageIndex)
    {
        var page = new TMap[PageSize];
        _pages[pageIndex] = page;
        return page;
    }
}
