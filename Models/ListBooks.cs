using Supabase.Postgrest.Attributes;  
using Supabase.Postgrest.Models;  
  
namespace MyMvcAuthProject.Models  
{  
    [Table("listbooks")]  
    public class ListBooks : BaseModel  
    {  
        [PrimaryKey("list_id")]  
        public Guid ListId { get; set; }  
  
        [PrimaryKey("book_id")]  
        public Guid BookId { get; set; }  
    }  
}