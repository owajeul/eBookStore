namespace eBookStore.Web.ViewModels;

public class OrderSuccessVM
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalPrice { get; set; }
}
