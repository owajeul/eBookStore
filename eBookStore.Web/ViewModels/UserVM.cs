using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Web.ViewModels
{
    public class UserVM
    {
        public string UserId { get; set; }  
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Enter a valid phone number.")]
        [Required]
        [DisplayName("Phone Number")]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address { get; set; }

    }
}
