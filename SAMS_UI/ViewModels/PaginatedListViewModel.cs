namespace SAMS_UI.ViewModels
{
    public class PaginatedListViewModel<T>
    {
        public List<T> Items
        { get; set; } = new();
        public int PageIndex
        { get; set; }
        public int PageSize
        { get; set; }
        public int TotalPages
        { get; set; }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public int TotalCount { get; internal set; }
    }
}
