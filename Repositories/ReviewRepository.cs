using MyMvcAuthProject.Models;  
using Supabase;  
using MyMvcAuthProject.Repositories;
  
namespace MyMvcAuthProject.Repositories  
{  
    public class ReviewRepository  
    {  
        private readonly Client _supabase;  
  
        public ReviewRepository(Client supabase)  
        {  
            _supabase = supabase;  
        }  
  
         public async Task<List<Review>> GetReviewsByBookId(Guid bookId)  
         {  
            var result = await _supabase  
               .From<Review>()  
               .Where(x => x.BookId == bookId)  
               .Get();  
               
            return result.Models;  
         }
        
    }  
}