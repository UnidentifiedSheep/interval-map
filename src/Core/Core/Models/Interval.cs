using Core.Enums;

namespace Core.Models;

public class Interval<T> where T : class
{ 
    public double Start { get; }
    public double End { get; }
    public IntervalBoundary StartBoundary { get; }
    public IntervalBoundary EndBoundary { get; }
    public T? Value { get; private set; }

    public Interval(double start, double end, T? value = null, IntervalBoundary startBoundary = IntervalBoundary.Inclusive,
        IntervalBoundary endBoundary = IntervalBoundary.Inclusive)
    {
        if(start < 0 || end < 0)
            throw new ArgumentException("Interval must be non-negative");
        if(start > end)
            throw new ArgumentException("Start must be less or equal to End");
        if (start.Equals(end) && (startBoundary == IntervalBoundary.Exclusive || endBoundary == IntervalBoundary.Exclusive))
            throw new ArgumentException("Invalid interval: for a zero-length interval, boundaries must be inclusive.");
        
        Start = start;
        End = end;
        Value = value;
        StartBoundary = startBoundary;
        EndBoundary = endBoundary;
    }

    public void ChangeValue(T? value) => Value = value;
    
    public bool IsNull => Value == null;

    public override bool Equals(object? obj)
    {
        if (obj is Interval<T> other)
            return Start.Equals(other.Start) && End.Equals(other.End);
        
        return false;
    }

    protected bool Equals(Interval other) => Start.Equals(other.Start) && End.Equals(other.End);
    

    public override int GetHashCode() => HashCode.Combine(Start, End);
    
}
public class Interval
{
    public double Start { get; }
    public double End { get; }
    public IntervalBoundary StartBoundary { get; }
    public IntervalBoundary EndBoundary { get; }
    public Interval(double start, double end, IntervalBoundary startBoundary = IntervalBoundary.Inclusive,
        IntervalBoundary endBoundary = IntervalBoundary.Inclusive)
    {
        if(start < 0 || end < 0)
            throw new ArgumentException("Interval must be non-negative");
        if(start > end)
            throw new ArgumentException("Start must be less or equal to End");
        if (start.Equals(end) && (startBoundary == IntervalBoundary.Exclusive || endBoundary == IntervalBoundary.Exclusive))
            throw new ArgumentException("Invalid interval: for a zero-length interval, boundaries must be inclusive.");

        Start = start;
        End = end;
        StartBoundary = startBoundary;
        EndBoundary = endBoundary;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Interval other)
            return Start.Equals(other.Start) && End.Equals(other.End);
        
        return false;
    }

    protected bool Equals(Interval other)
    {
        return Start.Equals(other.Start) && End.Equals(other.End);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }
}