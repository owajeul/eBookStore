using eBookStore.Application.DTOs;

namespace eBookStore.Web.ViewModels;

public class OrderVM
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public string UserId { get; set; }
    public string ShippingAddress { get; set; }
    public string PhoneNumber { get; set; }
    public List<OrderItemVM> OrderItems { get; set; }
    public decimal TotalPrice { get; set; }
}
