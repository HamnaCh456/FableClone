using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Models;
using MyMvcAuthProject.Data;
using Supabase;
using MyMvcAuthProject.Repositories;
using MyMvcAuthProject.ViewModels;
using System.Security.Claims;

namespace MyMvcAuthProject.Controllers
{
    public class PostController : Controller
    {
        private readonly Client _supabase;

        public PostController(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<IActionResult> Index()
        {
            // Initialize repositories
            var postRepository = new PostRepository(_supabase);
            var userProfileRepository = new UserProfileRepository(_supabase);

            // Get all posts
            var posts = await postRepository.GetAllPostsAsync();

            // Get user profile data for each post
            var userDisplayNames = new Dictionary<string, string>();
            var userImages = new Dictionary<string, string>();

            foreach (var post in posts)
            {
                if (!string.IsNullOrEmpty(post.UserId) && !userDisplayNames.ContainsKey(post.UserId))
                {
                    var userProfile = await userProfileRepository.GetUserByUserId(post.UserId);
                    if (userProfile != null)
                    {
                        userDisplayNames[post.UserId] = userProfile.DisplayName ?? "Anonymous User";
                        userImages[post.UserId] = userProfile.UserImage ?? "https://randomuser.me/api/portraits/lego/1.jpg";
                    }
                    else
                    {
                        userDisplayNames[post.UserId] = "Anonymous User";
                        userImages[post.UserId] = "https://randomuser.me/api/portraits/lego/1.jpg";
                    }
                }
            }

            ViewBag.Posts = posts;
            ViewBag.UserDisplayNames = userDisplayNames;
            ViewBag.UserImages = userImages;

            return View("AddPosts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPost(string postContent)
        {
            // Check if user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            // Get current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(postContent))
            {
                return BadRequest("Post content cannot be empty");
            }

            // Initialize repositories
            var postRepository = new PostRepository(_supabase);

            // Add the post
            await postRepository.AddPostAsync(userId, postContent);

            // Redirect to Index to show updated list
            return RedirectToAction("Index");
        }
    }
}
