    using Supabase.Postgrest.Attributes;  
    using Supabase.Postgrest.Models;  
    
    namespace MyMvcAuthProject.Models  
    {  
        [Table("list")]  
        public class List : BaseModel  
        {  
            [PrimaryKey("list_id")]  
            public Guid ListId { get; set; }  
    
            [Column("user_id")]  
            public string UserId { get; set; }  
    
            [Column("list_name")]  
            public string ListName { get; set; }  
        }  
    }