namespace KioskService.Core.DTO
{
    public class PaginatedList<T>
    {
        public int pagesCount { get; set; }
        public List<T> results { get; set; } = new List<T>();
    }
}
