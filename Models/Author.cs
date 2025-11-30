using Supabase.Postgrest.Attributes;  
using Supabase.Postgrest.Models;  
  
namespace MyMvcAuthProject.Models  
{  
    [Table("author")]  
    public class Author : BaseModel  
    {  
        [PrimaryKey("author_id")]  
        public Guid AuthorId { get; set; }  
  
        [Column("name")]  
        public string Name { get; set; }  
  
        [Column("bio")]  
        public string Bio { get; set; }  
    }  
}