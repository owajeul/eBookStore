using eBookStore.Domain.Entities;

namespace eBookStore.Web.ViewModels
{
    public class CartItemVM
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int BookId { get; set; }
        public BookVM Book { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
