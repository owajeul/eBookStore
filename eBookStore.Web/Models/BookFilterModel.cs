namespace eBookStore.Web.Models
{
    public class BookFilterModel
    {
        public string? Genre { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
    }

}
