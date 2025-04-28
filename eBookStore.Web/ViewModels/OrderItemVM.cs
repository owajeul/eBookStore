using eBookStore.Application.DTOs;

namespace eBookStore.Web.ViewModels;

public class OrderItemVM
{
    public int BookId { get; set; }
    public BookVM Book { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
