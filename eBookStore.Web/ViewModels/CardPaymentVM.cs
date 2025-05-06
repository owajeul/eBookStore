using System.ComponentModel.DataAnnotations;

namespace eBookStore.Web.ViewModels;

public class CardPaymentVM
{
    [Required]
    [Display(Name = "Card number")]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Card number must be 16 digits.")]
    public string CardNumber { get; set; }
    [Required]
    [Display(Name = "Expiry month")]
    public string ExpiryMonth { get; set; }
    [Required]
    [Display(Name = "Expiry year")]
    public string ExpiryYear { get; set; }
    [Required]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must be 3 or 4 digits.")]
    public string CVV { get; set; }
}
