# IntervalMap

[![NuGet Version](https://img.shields.io/nuget/v/IntervalMap.svg)](https://www.nuget.org/packages/IntervalMap/)  
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Adaptive interval mapping library for .NET. Efficiently stores and queries intervals;

---

## Features

- **Adaptive storage**: Automatically chooses the optimal backing type (`byte`, `ushort`, `int`) based on the number of intervals.  
- **Fast lookups**: O(1) access for checking if a value belongs to any interval.  
- **Intersection detection**: Quickly determine if a new interval overlaps with existing ones.  
- **Inclusive/Exclusive boundaries**: Supports both types of interval boundaries.  
- **Flexible generic values**: Store any reference type as interval values.

---

## Installation

Install via NuGet:

```bash
dotnet add package IntervalMap
```
## Usage

### Interval Map

```C#
var map = new IntervalMap<string, byte>(maxValue: 10, signsAfterComma: 2);

var interval1 = new Interval<string>(start: 2.23, end: 5.24, value: "First");
var interval2 = new Interval<string>(start: 6, end: 9, value: "Second");

map.AddInterval(interval1);
map.AddInterval(interval2);

// Check if value is within any interval
bool contains = map.Contains(3.23); // true

// Retrieve interval for a value
var interval = map.GetInterval(3);
Console.WriteLine(interval?.Value); // "First"

// Remove interval
map.RemoveInterval(interval1);

// Check intersections
bool hasIntersection = map.IsIntersectionExists(new Interval<string>(4, 7)); // true
```

### Adaptive Interval Map

AdaptiveIntervalMap<T> automatically grows its backing map as intervals are added beyond the current maximum value:

```C#
var adaptiveMap = new AdaptiveIntervalMap<string>(signsAfterComma: 2);
adaptiveMap.AddInterval(new Interval<string>(0, 5, "A"));
adaptiveMap.AddInterval(new Interval<string>(10, 15, "B")); // map grows automatically
```

### Interval Boundaries

Intervals can be inclusive or exclusive:

```C#
var exclusiveInterval = new Interval<string>(start: 5, end: 10, value: "X",
    startBoundary: IntervalBoundary.Exclusive,
    endBoundary: IntervalBoundary.Inclusive);
```
## Requirements

- .NET 8.0 or later 
- Supports any reference type for interval values.

## License

This project is licensed under the MIT License
