using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Models;
using MyMvcAuthProject.Data;
using Supabase;
using MyMvcAuthProject.Repositories;
using MyMvcAuthProject.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;


namespace MyMvcAuthProject.Controllers
{
    public class BookController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly Supabase.Client _supabase;

        public BookController(UserManager<IdentityUser> userManager, Supabase.Client supabase)
        {
            _userManager = userManager;
            _supabase = supabase;
        }
        [HttpGet("Book/{name?}")]
        public async Task<IActionResult> Index(string? name = null)
        {


            var bookRepository = new BookRepository(_supabase);
            
            Book book = null;
            if (!string.IsNullOrEmpty(name))
            {
                // Decode the name just in case, though ASP.NET Core usually handles it
                name = System.Net.WebUtility.UrlDecode(name);
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

            var authorRepositorry = new AuthorRepository(_supabase);
            var author = await authorRepositorry.GetAuthorByBookId(book.BookId); // Use fetched book's ID
            ViewBag.Author = author;

            var reviewRepository = new ReviewRepository(_supabase);
            var reviews = await reviewRepository.GetReviewsByBookId(book.BookId); // Use fetched book's ID
            ViewBag.Reviews = reviews;

            var userProfileRepository = new UserProfileRepository(_supabase);
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


            var bookRepository = new BookRepository(_supabase);
            var books = await bookRepository.SearchBooksByName(name);
            return PartialView("_BookSearchResults", books);
        }

        [HttpGet("Book/Search")]
        public IActionResult Search()
        {
            return View("SearchBooks");
        }

        [HttpGet("Book/ExploreBooks")]
        public async Task<IActionResult> ExploreBooks()
        {


            var bookRepository = new BookRepository(_supabase);
            var FictionBooks = await bookRepository.GetBooksByLabel("Fiction");
            ViewBag.FictionBooks = FictionBooks;
            var PhilosophyBooks = await bookRepository.GetBooksByLabel("Philosophy");
            ViewBag.PhilosophyBooks = PhilosophyBooks;
            var NonFictionBooks = await bookRepository.GetBooksByLabel("Non-Fiction");
            ViewBag.NonFictionBooks = NonFictionBooks;
            var PoliticsBooks = await bookRepository.GetBooksByLabel("Politics");
            ViewBag.PoliticsBooks = PoliticsBooks;
            return View();
            
        }

        [Authorize]
        [HttpPost("Book/AddBookToList")]
        public async Task<IActionResult> AddBookToList(Guid bookId, string listName)
        {

             
             var userId = _userManager.GetUserId(User);
             var userProfileRepository = new UserProfileRepository(_supabase);
             
             try
             {
                 await userProfileRepository.AddBookToListAsync(userId, bookId, listName);
                 return Ok("Book added to list successfully.");
             }
             catch (Exception ex)
             {
                 return BadRequest(ex.Message);
             }
        }
    }
}

