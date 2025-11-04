namespace CarRentalApi.Models;

public class PaginatedQuery
{
    private int _pageIndex = 1;
    private int _pageSize = 10;

    public int PageIndex
    {
        get => _pageIndex < 1 ? 1 : _pageIndex;
        set => _pageIndex = value;
    }
    public int PageSize
    {
        get => _pageSize < 1 ? 10 : _pageSize;
        set => _pageSize = value;
    }
}