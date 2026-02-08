using IntervalMap.Core.Models;

namespace IntervalMap.Core.Abstractions;

public abstract class IntervalMapBase<T>
{
    /// <summary>
    /// Количество знаков после запятой.
    /// </summary>
    public int MaxValue { get; protected set; }
    public byte SignsAfterComma { get; protected init; }
    public int PageSize { get; protected init; }
    public int ScaleFactor { get; protected init; }
    public bool IntersectionAllowed { get; protected init; } 
    
    protected int Scale(double value) => (int)(value * ScaleFactor);
    protected double DeScale(int value) => ((double)value / ScaleFactor);
    protected int GetPageIndex(int value) => value / PageSize;
    protected int GetPositionInPage(int value) => value % PageSize;
    

    /// <summary>
    /// Добавить интервал в карту.
    /// </summary>
    /// <param name="interval"></param>
    public abstract IntervalMapBase<T> AddInterval(T interval);

    /// <summary>
    /// Проверяет, содержит ли карта указанное значение.
    /// </summary>
    /// <param name="value">Число для проверки.</param>
    /// <returns>True, если значение принадлежит какому-либо интервалу, иначе False.</returns>
    public abstract bool Contains(double value);

    /// <summary>
    /// Возвращает интервал, которому принадлежит указанное значение.
    /// </summary>
    /// <param name="value">Число для поиска.</param>
    /// <returns>Интервал или null, если интервал не найден.</returns>
    public abstract T? GetInterval(double value);
    /// <summary>
    /// Удаление интервала с карты.
    /// </summary>
    /// <param name="interval"></param>
    /// <returns>True если удалось удалить интервал и False если не удалось.</returns>
    public abstract bool RemoveInterval(T interval);
    /// <summary>
    /// Проверка на то существует ли пересечения с другими интервалами.
    /// </summary>
    /// <param name="interval"></param>
    /// <returns>True если пересечение с уже существующим интервалом есть, в ином случае False.</returns>
    public abstract bool IsIntersectionExists(T interval);
}

public abstract class IntervalMapBase : IntervalMapBase<Interval>;