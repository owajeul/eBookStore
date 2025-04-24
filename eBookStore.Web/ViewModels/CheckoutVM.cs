using System.ComponentModel.DataAnnotations;
using eBookStore.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace eBookStore.Web.ViewModels
{
    public class CheckoutVM
    {
        [Required]
        [Display(Name = "Shipping address")]
        public string ShippingAddress { get; set; }
        [Required]
        [Display(Name = "Phone number")]
        [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Enter a valid phone number.")]
        public string PhoneNumber { get; set; }
        [ValidateNever]
        public List<CartItem> Items { get; set; }
    }
}
