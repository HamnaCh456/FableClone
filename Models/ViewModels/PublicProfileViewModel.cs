using MyMvcAuthProject.Models;

namespace MyMvcAuthProject.ViewModels
{
    public class PublicProfileViewModel
    {
        public UserProfile Profile { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Post> Posts { get; set; }
        public Dictionary<string, List<Book>> BooksLists { get; set; }

        public PublicProfileViewModel()
        {
            Reviews = new List<Review>();
            Posts = new List<Post>();
            BooksLists = new Dictionary<string, List<Book>>();
        }
    }
}
