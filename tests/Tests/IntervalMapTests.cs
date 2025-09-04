using IntervalMap.Core.Enums;
using IntervalMap.Core.Models;
using IntervalMap.Variations;

namespace Tests;

public class IntervalMapTests
{
    [Theory]
    [InlineData(IntervalBoundary.Inclusive, IntervalBoundary.Inclusive, 0, 5, 5)]
    [InlineData(IntervalBoundary.Inclusive, IntervalBoundary.Exclusive, 0, 5, 4)]
    [InlineData(IntervalBoundary.Exclusive, IntervalBoundary.Inclusive, 0, 5, 5)]
    [InlineData(IntervalBoundary.Exclusive, IntervalBoundary.Exclusive, 0, 5, 4)]
    public void AddInterval_ShouldHandleBoundariesCorrectly(
        IntervalBoundary startBoundary, IntervalBoundary endBoundary, double start, double end, double testValue)
    {
        var map = new IntervalMap<string, byte>(10);
        var interval = new Interval<string>(start, end, "Value", startBoundary, endBoundary);

        map.AddInterval(interval);

        bool contains = map.Contains(testValue);
        bool expected = !(startBoundary == IntervalBoundary.Exclusive && testValue.Equals(start)) &&
                        !(endBoundary == IntervalBoundary.Exclusive && testValue.Equals(end));

        Assert.Equal(expected, contains);
    }

    [Fact]
    public void AddInterval_ShouldThrow_WhenIntersectingExistingInterval()
    {
        var map = new IntervalMap<string, byte>(10);
        map.AddInterval(new Interval<string>(2, 6, "A"));

        Assert.Throws<ArgumentException>(() =>
            map.AddInterval(new Interval<string>(5, 8, "B")));
    }

    [Fact]
    public void RemoveInterval_ShouldNotAffectOtherIntervals()
    {
        var map = new IntervalMap<string, byte>(10);
        var interval1 = new Interval<string>(0, 3, "A");
        var interval2 = new Interval<string>(4, 6, "B");

        map.AddInterval(interval1);
        map.AddInterval(interval2);

        map.RemoveInterval(interval1);

        Assert.Null(map.GetInterval(2));
        Assert.Equal("B", map.GetInterval(5)?.Value);
    }

    [Fact]
    public void GetInterval_ShouldReturnCorrectForEdgeCases()
    {
        var map = new IntervalMap<string, byte>(10);
        var interval = new Interval<string>(0, 10, "Full");

        map.AddInterval(interval);

        Assert.Equal("Full", map.GetInterval(0)?.Value);
        Assert.Equal("Full", map.GetInterval(10)?.Value);
        Assert.Null(map.GetInterval(11));
    }

    [Fact]
    public void AddInterval_ShouldWorkWithScaledValues()
    {
        var map = new IntervalMap<string, byte>(1, signsAfterComma: 2); // scale factor 100
        var interval = new Interval<string>(0.01, 0.05, "Small");

        map.AddInterval(interval);

        Assert.True(map.Contains(0.03));
        Assert.False(map.Contains(0.06));
    }

    [Fact]
    public void MultipleIntervals_ShouldMapCorrectlyWithDifferentTypes()
    {
        var mapByte = new IntervalMap<string, byte>(20);
        var mapUShort = new IntervalMap<string, ushort>(20);

        var interval1 = new Interval<string>(1, 5, "A");
        var interval2 = new Interval<string>(6, 10, "B");

        mapByte.AddInterval(interval1);
        mapByte.AddInterval(interval2);

        mapUShort.AddInterval(interval1);
        mapUShort.AddInterval(interval2);

        Assert.Equal("A", mapByte.GetInterval(3)?.Value);
        Assert.Equal("B", mapByte.GetInterval(7)?.Value);

        Assert.Equal("A", mapUShort.GetInterval(3)?.Value);
        Assert.Equal("B", mapUShort.GetInterval(7)?.Value);
    }

    [Fact]
    public void IsIntersectionExists_ShouldDetectMultipleIntersectingIntervals()
    {
        var map = new IntervalMap<string, byte>(20);
        map.AddInterval(new Interval<string>(1, 5, "X"));
        map.AddInterval(new Interval<string>(6, 10, "Y"));

        Assert.True(map.IsIntersectionExists(new Interval<string>(4, 7, "Intersect")));
        Assert.False(map.IsIntersectionExists(new Interval<string>(11, 12, "NoIntersect")));
    }

    [Fact]
    public void RemoveInterval_ShouldReturnFalse_WhenIntervalNotFound()
    {
        var map = new IntervalMap<string, byte>(10);
        var interval = new Interval<string>(1, 3, "A");

        Assert.False(map.RemoveInterval(interval));
    }

    [Fact]
    public void AddingTooManyIntervals_ShouldThrowOverflow()
    {
        var map = new IntervalMap<string, byte>(100);
        double start = 0;
        var end = start + 0.1;
        for (int i = 0; i < 256; i++)
        {
            map.AddInterval(new Interval<string>(start, end, i.ToString()));
            start = end + 0.1;
            end = start + 0.1;
        }

        Assert.Throws<InvalidOperationException>(() =>
            map.AddInterval(new Interval<string>(start, end, "Overflow")));
    }
        
    [Fact]
    public void AddInterval_ShouldContainValueWithinInterval()
    {
        var map = new IntervalMap<string, byte>(10);
        var interval = new Interval<string>(2, 5, "Test");

        map.AddInterval(interval);

        Assert.True(map.Contains(3));
        Assert.True(map.Contains(5));
        Assert.False(map.Contains(6));
    }

    [Fact]
    public void GetInterval_ShouldReturnCorrectInterval()
    {
        var map = new IntervalMap<string, byte>(10);
        var interval = new Interval<string>(1, 4, "A");

        map.AddInterval(interval);

        var result = map.GetInterval(2);
        Assert.NotNull(result);
        Assert.Equal("A", result.Value);

        Assert.Null(map.GetInterval(5));
    }
    
    [Fact]
    public void RemoveInterval_ShouldRemoveInterval()
    {
        var map = new IntervalMap<string, byte>(10);
        var interval = new Interval<string>(0, 3, "X");

        map.AddInterval(interval);
        Assert.True(map.Contains(2));

        map.RemoveInterval(interval);
        Assert.False(map.Contains(2));
        Assert.Null(map.GetInterval(2));
    }

    [Fact]
    public void IsIntersectionExists_ShouldDetectIntersection()
    {
        var map = new IntervalMap<string, byte>(10);
        var interval1 = new Interval<string>(1, 4, "A");
        var interval2 = new Interval<string>(3, 6, "B");

        map.AddInterval(interval1);

        Assert.True(map.IsIntersectionExists(interval2));
        Assert.False(map.IsIntersectionExists(new Interval<string>(5, 7, "C")));
    }

    [Fact]
    public void AddMultipleIntervals_ShouldHandleMultipleCorrectly()
    {
        var map = new IntervalMap<string, byte>(20);
        var interval1 = new Interval<string>(1, 5, "First");
        var interval2 = new Interval<string>(6, 10, "Second");

        map.AddInterval(interval1);
        map.AddInterval(interval2);

        Assert.Equal("First", map.GetInterval(3)?.Value);
        Assert.Equal("Second", map.GetInterval(7)?.Value);
        Assert.Null(map.GetInterval(11));
    }

    [Fact]
    public void AddInterval_ThrowsException_WhenExceedsMaxValue()
    {
        var map = new IntervalMap<string, byte>(5);
        var interval = new Interval<string>(0, 10, "TooBig");

        Assert.Throws<ArgumentOutOfRangeException>(() => map.AddInterval(interval));
    }
}