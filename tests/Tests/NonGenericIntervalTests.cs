using IntervalMap.Core.Enums;
using IntervalMap.Core.Models;

namespace Tests;

public class NonGenericIntervalTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var interval = new Interval(1, 5, IntervalBoundary.Inclusive, IntervalBoundary.Exclusive);

        Assert.Equal(1, interval.Start);
        Assert.Equal(5, interval.End);
        Assert.Equal(IntervalBoundary.Inclusive, interval.StartBoundary);
        Assert.Equal(IntervalBoundary.Exclusive, interval.EndBoundary);
    }

    [Fact]
    public void Constructor_ShouldThrow_OnInvalidIntervals()
    {
        Assert.Throws<ArgumentException>(() => new Interval(-1, 5));
        Assert.Throws<ArgumentException>(() => new Interval(5, -1));
        Assert.Throws<ArgumentException>(() => new Interval(6, 5));
        Assert.Throws<ArgumentException>(() => new Interval(5, 5, IntervalBoundary.Exclusive));
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameIntervals()
    {
        var a = new Interval(1, 3);
        var b = new Interval(1, 3);
        var c = new Interval(2, 3);

        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
    }

    [Fact]
    public void GetHashCode_ShouldBeSame_ForEqualIntervals()
    {
        var a = new Interval(1, 3);
        var b = new Interval(1, 3);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }
}