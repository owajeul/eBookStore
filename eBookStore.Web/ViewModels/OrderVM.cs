using eBookStore.Application.DTOs;

namespace eBookStore.Web.ViewModels;

public class OrderVM
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public string UserId { get; set; }
    public AddressDto ShippingAddress { get; set; }
    public AddressDto BillingAddress { get; set; }
    public string PaymentMethod { get; set; }
    public string PaymentStatus { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
    public decimal TotalPrice { get; set; }
}
