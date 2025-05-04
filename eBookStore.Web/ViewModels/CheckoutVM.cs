using System.ComponentModel.DataAnnotations;
using eBookStore.Domain.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace eBookStore.Web.ViewModels;

public class CheckoutVM
{
    [Display(Name = "Shipping address")]
    public AddressVM ShippingAddress { get; set; }
    public AddressVM BillingAddress { get; set; }
    public bool BillingSameAsShipping { get; set; }
    public PaymentVM Payment { get; set; }
    [ValidateNever]
    public List<CartItemVM> Items { get; set; }
}
