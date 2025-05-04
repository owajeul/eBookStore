using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Web.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }

        [DisplayName("Phone number")]
        [Required]
        [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Enter a valid phone number.")]
        public string PhoneNumber { get; set; }
        public string ?RedirectUrl { get; set; }
    }
}
