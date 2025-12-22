using Microsoft.EntityFrameworkCore;
using MyMvcAuthProject.Data;
using MyMvcAuthProject.Models;
using System.Threading.Tasks;
using Supabase;

namespace MyMvcAuthProject.Repositories
{
    public class UserProfileRepository
    {
        private readonly Client _supabase;

        public UserProfileRepository(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<UserProfile?> GetUserByUserId(string userId)  
        {  
            var result = await _supabase  
               .From<UserProfile>()  
               .Where(x => x.UserId == userId)  
               .Get();  
               
            return result.Models.FirstOrDefault();  
        }
        public async Task<UserProfile?> SaveUserData(UserProfile userProfile)  
        {  
            var result = await _supabase  
                .From<UserProfile>()  
                .Upsert(userProfile);  
                
            return result.Models.FirstOrDefault();  
        }
       
    }
}
