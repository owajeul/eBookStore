using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Web.ViewModels;

public class AddressVM
{
    [Required]
    public string Name { get; set; }
    [Required]
    [DisplayName("Street address")]
    public string StreetAddress { get; set; }
    [Required]
    public string City { get; set; }
    [Required]
    [DisplayName("Post code")]
    public string PostCode { get; set; }
    [Required]
    [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Enter a valid phone number.")]
    [DisplayName("Phone number")]
    public string Phone { get; set; }
}
