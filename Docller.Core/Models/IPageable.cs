namespace Docller.Core.Models
{
    public interface IPageable
    {
        int PageSize { get; }
        int PageNumber { get; }
        int TotalPages { get; }
        bool HasPreviousPage { get; }
        bool HasNextPage { get; }
        bool HasPages { get; }
        int GetPreviousPage();
        int GetNextPage();
        bool IsActivePage(int pageNumber);
    }
}