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
        public async Task<Review> AddReview(Guid bookId, string userId, int? rating, string reviewContent)  
        {  
            var newReview = new Review  
            {  
                ReviewId = Guid.NewGuid(),  
                BookId = bookId,  
                UserId = userId,  
                Rating = rating,  
                ReviewContent = reviewContent  
            };  
        
                var result = await _supabase  
                    .From<Review>()  
                    .Insert(newReview, new QueryOptions { Returning = ReturnType.Representation });  
                
                return result.Models.FirstOrDefault();  
        }
        
    }  
}