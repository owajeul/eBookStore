using eBookStore.Domain.Entities;

namespace eBookStore.Web.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Book> Books { get; set; }    
        public IEnumerable<string> BookCategories { get; set; }
        public string? SelectedCategory { get; set; }
    }
}
