namespace CarRentalApi.Models;

public class PaginatedList<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; private set; }
    public int PageIndex { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}