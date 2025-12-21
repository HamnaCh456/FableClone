using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Attributes;  
using Supabase.Postgrest.Models;  
  
namespace MyMvcAuthProject.Models  
{  
    [Table("userprofile")]  
    public class UserProfile : BaseModel  
    {  
        [Key]
        [PrimaryKey("user_id")]  
        public string UserId { get; set; }  
  
        [Column("display_name")]  
        public string DisplayName { get; set; }  
  
        [Column("bio")]  
        public string Bio { get; set; }  
  
        [Column("user_image")]  
        public string UserImage { get; set; }  
    }  
}