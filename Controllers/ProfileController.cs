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
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser>_userManager;
        private readonly Client _supabase;


        public ProfileController(UserManager<IdentityUser> userManager)
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
        [Authorize]
        public async Task<IActionResult> UserProfile()
        {
            var userId = _userManager.GetUserId(User);
            var userProfileRepository = new UserProfileRepository(_supabase);
            var userProfile = await userProfileRepository.GetUserByUserId(userId);
            
            if (userProfile == null)
            {
                userProfile = new UserProfile
                {
                    UserId = userId,
                    DisplayName = "Reader",
                    Bio = "",
                    UserImage = "https://randomuser.me/api/portraits/lego/1.jpg" // Default placeholder
                };
                
                // Optionally save this default profile immediately or just let the user save it later.
                // For now, let's just pass it to the view so there's no crash.
            }

            return View("UserProfile",userProfile);
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(UserProfile user)
        {
            var userId = _userManager.GetUserId(User);
            var userProfileRepository = new UserProfileRepository(_supabase);
            
            // Fetch existing profile to preserve the Image URL if not provided (though model binding might overwrite it with null if not handled)
            // Ideally we should have a view model, but using the domain model directly as requested.
            var existingProfile = await userProfileRepository.GetUserByUserId(userId);
            
            if (existingProfile != null)
            {
                // Update existing profile (preserve image)
                user.UserImage = existingProfile.UserImage;
            }
            else
            {
                // Initialize default values for new profile
                user.UserImage = "https://randomuser.me/api/portraits/lego/1.jpg";
            }
            
            // Always ensure UserId is set correctly
            user.UserId = userId; 
            
            // Upsert (Insert or Update)
            await userProfileRepository.SaveUserData(user);

            return RedirectToAction("UserProfile");
        }
    }
}
