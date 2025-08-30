using Core.Enums;
using Core.Models;
using IntervalMap.Variations;

namespace Tests
{
    public class AdaptiveIntervalTests
    {
        [Fact]
        public void AddInterval_ShouldContainValueWithinInterval()
        {
            var map = new AdaptiveIntervalMap<string>();
            var interval = new Interval<string>(2, 5, "Test");

            map.AddInterval(interval);

            Assert.True(map.Contains(3));
            Assert.True(map.Contains(5));
            Assert.False(map.Contains(6));
        }

        [Fact]
        public void GetInterval_ShouldReturnCorrectInterval()
        {
            var map = new AdaptiveIntervalMap<string>();
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
            var map = new AdaptiveIntervalMap<string>();
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
            var map = new AdaptiveIntervalMap<string>();
            var interval1 = new Interval<string>(1, 4, "A");
            var interval2 = new Interval<string>(3, 6, "B");

            map.AddInterval(interval1);

            Assert.True(map.IsIntersectionExists(interval2));
            Assert.False(map.IsIntersectionExists(new Interval<string>(5, 7, "C")));
        }

        [Fact]
        public void AddMultipleIntervals_ShouldHandleMultipleCorrectly()
        {
            var map = new AdaptiveIntervalMap<string>();
            var interval1 = new Interval<string>(1, 5, "First");
            var interval2 = new Interval<string>(6, 10, "Second");

            map.AddInterval(interval1);
            map.AddInterval(interval2);

            Assert.Equal("First", map.GetInterval(3)?.Value);
            Assert.Equal("Second", map.GetInterval(7)?.Value);
            Assert.Null(map.GetInterval(11));
        }

        [Fact]
        public void AddInterval_ShouldGrowInternalMap_WhenExceedsMaxValue()
        {
            var map = new AdaptiveIntervalMap<string>();
            var interval1 = new Interval<string>(0, 1, "A");
            var interval2 = new Interval<string>(1000, 1010, "B");

            map.AddInterval(interval1);
            map.AddInterval(interval2);

            Assert.True(map.Contains(1005));
            Assert.Equal("B", map.GetInterval(1005)?.Value);
        }

        [Theory]
        [InlineData(IntervalBoundary.Inclusive, IntervalBoundary.Inclusive, 0, 5, 5)]
        [InlineData(IntervalBoundary.Inclusive, IntervalBoundary.Exclusive, 0, 5, 4)]
        [InlineData(IntervalBoundary.Exclusive, IntervalBoundary.Inclusive, 0, 5, 5)]
        [InlineData(IntervalBoundary.Exclusive, IntervalBoundary.Exclusive, 0, 5, 4)]
        public void AddInterval_ShouldHandleBoundariesCorrectly(
            IntervalBoundary startBoundary,
            IntervalBoundary endBoundary,
            double start, double end,
            double testValue)
        {
            var map = new AdaptiveIntervalMap<string>();
            var interval = new Interval<string>(start, end, "Value", startBoundary, endBoundary);

            map.AddInterval(interval);

            bool contains = map.Contains(testValue);
            bool expected = !(startBoundary == IntervalBoundary.Exclusive && testValue.Equals(start)) &&
                            !(endBoundary == IntervalBoundary.Exclusive && testValue.Equals(end));

            Assert.Equal(expected, contains);
        }

        [Fact]
        public void RemoveInterval_ShouldReturnFalse_WhenIntervalNotFound()
        {
            var map = new AdaptiveIntervalMap<string>();
            var interval = new Interval<string>(1, 3, "A");

            Assert.False(map.RemoveInterval(interval));
        }

        [Fact]
        public void MultipleIntervals_ShouldMaintainOrderAndValues()
        {
            var map = new AdaptiveIntervalMap<string>();
            for (int i = 0; i < 50; i++)
            {
                map.AddInterval(new Interval<string>(i * 2, i * 2 + 1, $"Interval{i}"));
            }

            for (int i = 0; i < 50; i++)
            {
                Assert.Equal($"Interval{i}", map.GetInterval(i * 2)?.Value);
            }
        }

        [Fact]
        public void AdaptiveMap_ShouldScaleWithSignsAfterComma()
        {
            var map = new AdaptiveIntervalMap<string>(3); // 3 decimal digits
            var interval = new Interval<string>(0.001, 0.005, "Small");

            map.AddInterval(interval);

            Assert.True(map.Contains(0.003));
            Assert.False(map.Contains(0.006));
        }

        [Fact]
        public void AddingManyIntervals_ShouldSwitchInternalMapTypes()
        {
            var map = new AdaptiveIntervalMap<string>();

            var start = 0.1;
            var end = start + 1;
            for (int i = 0; i < 300; i++)
            {
                map.AddInterval(new Interval<string>(start, end, $"Interval{i}"));
                start = end + 1;
                end = start + 1;
            }

            Assert.Equal("Interval0", map.GetInterval(0.3)?.Value);
            Assert.Equal("Interval299", map.GetInterval(598.5)?.Value);
        }
        
    }
}
