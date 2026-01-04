namespace DemoApi.Domain.Models
{
    public class PagedResult<T>
    {
        // List of Items
        public List<T> Items { get; set; } = new List<T>();
        // Total nums of pokemon found
        public int TotalCount { get; set; }
        // Current page num
        public int Page { get; set; }
        // How many can show on one page
        public int PageSize { get; set; }

        // Total pages calculation
        // If total 105 Eevee, and we can only show 10 on each page then
        // we have 105 / 10  round up = > 11 pages => total pages
        public int TotalPages => 
            (int)Math.Ceiling(
                (double)TotalCount / PageSize
                );
    }
}
