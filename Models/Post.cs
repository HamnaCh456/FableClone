using Supabase.Postgrest.Attributes;  
using Supabase.Postgrest.Models;  
  
namespace MyMvcAuthProject.Models  
{  
    [Table("posts")]  
    public class Post : BaseModel  
    {  
        [PrimaryKey("post_id", false)]
        [Column("post_id")]
        public Guid PostId { get; set; }  
  
        [Column("user_id")]  
        public string UserId { get; set; }  
  
        [Column("post_content")]  
        public string PostContent { get; set; }  
    }  
}