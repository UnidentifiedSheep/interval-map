using Core.Enums;
using Core.Models;

namespace Tests;

public class GenericIntervalTests
{
    [Fact]
    public void Constructor_ShouldInitializeProperties()
    {
        var interval = new Interval<string>(1, 5, "Value", IntervalBoundary.Inclusive, IntervalBoundary.Exclusive);
        
        Assert.Equal(1, interval.Start);
        Assert.Equal(5, interval.End);
        Assert.Equal(IntervalBoundary.Inclusive, interval.StartBoundary);
        Assert.Equal(IntervalBoundary.Exclusive, interval.EndBoundary);
        Assert.Equal("Value", interval.Value);
        Assert.False(interval.IsNull);
    }

    [Fact]
    public void Constructor_ShouldThrow_OnInvalidIntervals()
    {
        Assert.Throws<ArgumentException>(() => new Interval<string>(-1, 5));
        Assert.Throws<ArgumentException>(() => new Interval<string>(5, -1));
        Assert.Throws<ArgumentException>(() => new Interval<string>(6, 5));
        Assert.Throws<ArgumentException>(() => new Interval<string>(5, 5, "X", IntervalBoundary.Exclusive));
    }

    [Fact]
    public void ChangeValue_ShouldUpdateValue()
    {
        var interval = new Interval<string>(1, 2, "A");
        interval.ChangeValue("B");
        Assert.Equal("B", interval.Value);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameIntervals()
    {
        var a = new Interval<string>(1, 3);
        var b = new Interval<string>(1, 3);
        var c = new Interval<string>(2, 3);

        Assert.True(a.Equals(b));
        Assert.False(a.Equals(c));
    }

    [Fact]
    public void GetHashCode_ShouldBeSame_ForEqualIntervals()
    {
        var a = new Interval<string>(1, 3);
        var b = new Interval<string>(1, 3);

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void IsNull_ShouldBeTrue_WhenValueIsNull()
    {
        var interval = new Interval<string>(0, 1);
        Assert.True(interval.IsNull);
    }
}