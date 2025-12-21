using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Models;
using MyMvcAuthProject.Data;
using Supabase;
using MyMvcAuthProject.Repositories;
using MyMvcAuthProject.ViewModels;

namespace MyMvcAuthProject.Controllers
{
    public class BookController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var url = "https://phqjkkhovqndiyyuwljc.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBocWpra2hvdnFuZGl5eXV3bGpjIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2MzExNDc0MywiZXhwIjoyMDc4NjkwNzQzfQ.ZPEqacRXPHk1FdJPMfbGohMyTW0oIpnxuPrzQePlLVI";

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            var supabase = new Client(url, key, options);
            await supabase.InitializeAsync();

            var bookRepository = new BookRepository(supabase);
            var books = await bookRepository.GetBookById();
            var authorRepositorry = new AuthorRepository(supabase);
            var author = await authorRepositorry.GetAuthorByBookId( Guid.Parse("4c703670-360f-4ade-8644-37de2200993e"));
            ViewBag.Author = author;
            var reviewRepository = new ReviewRepository(supabase);
            var reviews = await reviewRepository.GetReviewsByBookId(Guid.Parse("4c703670-360f-4ade-8644-37de2200993e"));
            ViewBag.Reviews = reviews;

            var userProfileRepository = new UserProfileRepository(supabase);
            var userDisplayNames = new Dictionary<string, string>();
            var userImages = new Dictionary<string, string>();

            if (reviews != null)
            {
                foreach (var review in reviews)
                {
                    if (!string.IsNullOrEmpty(review.UserId) && !userDisplayNames.ContainsKey(review.UserId))
                    {
                        var user = await userProfileRepository.GetUserByUserId(review.UserId);
                        userDisplayNames[review.UserId] = user != null ? user.DisplayName : "Unknown User";
                        userImages[review.UserId] = user != null && !string.IsNullOrEmpty(user.UserImage) 
                            ? user.UserImage 
                            : "https://randomuser.me/api/portraits/women/44.jpg"; // fallback image
                    }
                }
            }
            ViewBag.UserDisplayNames = userDisplayNames;
            ViewBag.UserImages = userImages;

            return View("Index",books.FirstOrDefault());
        }

        
    }
}

