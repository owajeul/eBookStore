using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace eBookStore.Web.ViewModels
{
    public class BookVM
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        [DisplayName("Image url")]
        public string ImageUrl { get; set; }
        [Required]
        public int Stock { get; set; }
    }
}
