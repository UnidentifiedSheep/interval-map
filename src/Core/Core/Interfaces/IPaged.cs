namespace Core.Interfaces;

/// <typeparam name="T">Page value type</typeparam>
public interface IPaged<T>
{
    /// <returns><b>Null</b> - if page not exists, Array or page values if exists</returns>
    T[]? GetPage(int pageIndex);
}