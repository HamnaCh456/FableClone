using Supabase.Postgrest.Attributes;  
using Supabase.Postgrest.Models;  
  
namespace MyMvcAuthProject.Models  
{  
    [Table("reviews")]  
    public class Review : BaseModel  
    {  
        [PrimaryKey("review_id")]  
        public Guid ReviewId { get; set; }  
  
        [Column("user_id")]  
        public string UserId { get; set; }  
  
        [Column("book_id")]  
        public Guid BookId { get; set; }  
  
        [Column("rating")]  
        public int? Rating { get; set; }  
  
        [Column("review_content")]  
        public string ReviewContent { get; set; }  
    }  
}