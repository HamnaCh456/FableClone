using MyMvcAuthProject.Models;
using Supabase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMvcAuthProject.Repositories
{
    public class PostRepository
    {
        private readonly Client _supabase;

        public PostRepository(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<Post> AddPostAsync(string userId, string postContent)  
        {  
            var post = new Post  
            {  
                PostId = Guid.NewGuid(),  
                UserId = userId,  
                PostContent = postContent  
            };  
      
            var response = await _supabase.From<Post>().Insert(post);  
            return response.Models.FirstOrDefault() ?? post;  
        }
        public async Task<List<Post>> GetAllPostsAsync()  
        {  
            var response = await _supabase.From<Post>().Get();  
            return response.Models;  
        }
         public async Task<List<Post>> GetUserPostsAsync(string userId)  
         {  
            var response = await _supabase  
                  .From<Post>()  
                  .Where(x => x.UserId == userId)  
                  .Get();  
               
            return response.Models;  
      }  
    }
}
