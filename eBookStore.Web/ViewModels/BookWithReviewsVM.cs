namespace eBookStore.Web.ViewModels
{
    public class BookWithReviewsVM: BookVM
    {
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
        public List<BookReviewVM> Reviews { get; set; } = new List<BookReviewVM>();
    }
}
