using eBookStore.Web.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class PaymentVM
{
    [Required(ErrorMessage = "Payment method is required.")]
    public string Method { get; set; }
    public CardPaymentVM CardPayment { get; set; } = new CardPaymentVM();
}
