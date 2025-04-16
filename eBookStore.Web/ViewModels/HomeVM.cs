using eBookStore.Domain.Entities;

namespace eBookStore.Web.ViewModels
{
    public class HomeVM
    {
        public IEnumerable<Book> Books { get; set; }    
        public IEnumerable<string> BookGenres { get; set; }
        public string? SelectedGenre { get; set; }
    }
}
