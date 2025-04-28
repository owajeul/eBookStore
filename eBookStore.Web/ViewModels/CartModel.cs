using System.ComponentModel.DataAnnotations;

namespace eBookStore.Web.ViewModels
{
    public class CartVM
    {
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public List<CartItemVM> CartItems { get; set; } = new List<CartItemVM>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
