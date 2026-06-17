namespace Hospital_Management_System.DTOs
{
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages =>
            PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);
    }

    public class PagedQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? Search { get; set; }

        public int NormalizedPage => Page < 1 ? 1 : Page;
        public int NormalizedPageSize => PageSize switch
        {
            <= 0 => 20,
            > 200 => 200,
            _ => PageSize
        };
    }
}
