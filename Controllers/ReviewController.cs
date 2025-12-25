using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using MyMvcAuthProject.Models;
using MyMvcAuthProject.Data;
using Supabase;
using MyMvcAuthProject.Repositories;
using MyMvcAuthProject.ViewModels;

namespace MyMvcAuthProject.Controllers
{
    public class ReviewController : Controller
    {
        private readonly Supabase.Client _supabase;
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
            var url = "https://phqjkkhovqndiyyuwljc.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBocWpra2hvdnFuZGl5eXV3bGpjIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2MzExNDc0MywiZXhwIjoyMDc4NjkwNzQzfQ.ZPEqacRXPHk1FdJPMfbGohMyTW0oIpnxuPrzQePlLVI";

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _supabase = new Supabase.Client(url, key, options);
        }

        private async Task EnsureSupabaseInitialized()
        {
            await _supabase.InitializeAsync();
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview([FromForm] Guid bookId, [FromForm] int rating, [FromForm] string reviewContent)
        {
            if (bookId == Guid.Empty)
            {
                return BadRequest("Invalid Book ID.");
            }

            if (string.IsNullOrWhiteSpace(reviewContent))
            {
                return BadRequest("Review content cannot be empty.");
            }

            try
            {
                await EnsureSupabaseInitialized();

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("user id is null");
                }

                var reviewRepository = new ReviewRepository(_supabase);
                var result = await reviewRepository.AddReview(bookId, userId, rating, reviewContent);

                if (result == null)
                {
                    // If result is null, maybe the insert failed or returned nothing
                    return StatusCode(500, "Failed to save the review. Please try again.");
                }

                // Fetch update reviews and user metadata for the partial view
                var reviews = await reviewRepository.GetReviewsByBookId(bookId);
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
                                : "https://randomuser.me/api/portraits/women/44.jpg";
                        }
                    }
                }
                ViewBag.UserDisplayNames = userDisplayNames;
                ViewBag.UserImages = userImages;

                var bookRepository = new BookRepository(_supabase);
                var books = await bookRepository.GetBookById(bookId.ToString());
                var book = books.FirstOrDefault();

                // It's important to return the partial view that will be injected into the existing page
                return PartialView("~/Views/Book/_ReviewPartial.cshtml", book);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"AddReview Error: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
