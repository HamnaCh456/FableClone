using System.ComponentModel.DataAnnotations;

namespace MyMvcAuthProject.Models.ViewModels
{
    public class AdminAddBookViewModel
    {
        // Book properties
        [Required]
        [Display(Name = "Book Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Display(Name = "Rating")]
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public double Rating { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Label")]
        public string Label { get; set; }

        [Display(Name = "Book URL")]
        public string? BookURL { get; set; }

        // Author properties
        [Required]
        [Display(Name = "Author Name")]
        public string AuthorName { get; set; }

        [Display(Name = "Author Bio")]
        public string AuthorBio { get; set; }

        [Display(Name = "Author Image URL")]
        public string AuthorImage { get; set; }
    }
}
