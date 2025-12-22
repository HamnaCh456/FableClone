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
        private readonly Client _supabase;
         private readonly UserManager<IdentityUser>_userManager;
        public ReviewController(UserManager<IdentityUser> userManager)
        {
           _userManager = userManager;
            var url = "https://phqjkkhovqndiyyuwljc.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBocWpra2hvdnFuZGl5eXV3bGpjIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2MzExNDc0MywiZXhwIjoyMDc4NjkwNzQzfQ.ZPEqacRXPHk1FdJPMfbGohMyTW0oIpnxuPrzQePlLVI";

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _supabase = new Client(url, key, options);
            _supabase.InitializeAsync().Wait();
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(Guid bookId, int rating, string reviewContent)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var reviewRepository = new ReviewRepository(_supabase);
            await reviewRepository.AddReview(bookId, userId, rating, reviewContent);

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

            return PartialView("_ReviewPartial");
        }
    }
}
