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
        public string ?PhoneNumber { get; set; }
        public string ?RedirectUrl { get; set; }
    }
}
