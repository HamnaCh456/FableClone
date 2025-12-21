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
        [HttpGet("Book/{name?}")]
        public async Task<IActionResult> Index(string name = null)
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
            
            Book book = null;
            if (!string.IsNullOrEmpty(name))
            {
                // Decode the name just in case, though ASP.NET Core usually handles it
                var searchResults = await bookRepository.SearchBooksByName(name);
                book = searchResults.FirstOrDefault();
            }

            // Fallback if no name provided or book not found
            if (book == null)
            {
                var books = await bookRepository.GetBookById(); 
                book = books.FirstOrDefault();
            }

            if (book == null)
            {
                return NotFound();
            }

            var authorRepositorry = new AuthorRepository(supabase);
            var author = await authorRepositorry.GetAuthorByBookId(book.BookId); // Use fetched book's ID
            ViewBag.Author = author;

            var reviewRepository = new ReviewRepository(supabase);
            var reviews = await reviewRepository.GetReviewsByBookId(book.BookId); // Use fetched book's ID
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

            return View("Index", book);
        }
        
        [Route("Book/Store")]
        public async Task<IActionResult> Store(string name)
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
            var books = await bookRepository.SearchBooksByName(name);
            return PartialView("_BookSearchResults", books);
        }

        [HttpGet("Book/Search")]
        public IActionResult Search()
        {
            return View("SearchBooks");
        }

    }
}

