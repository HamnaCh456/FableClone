using Supabase.Postgrest.Attributes;  
using Supabase.Postgrest.Models;  

namespace MyMvcAuthProject.Models{
[Table("book")]  
public class Book : BaseModel  
{  
    [PrimaryKey("book_id")]  
    public Guid BookId { get; set; }  
  
    [Column("title")]  
    public string Title { get; set; }  
  
    [Column("price")]  
    public decimal Price { get; set; }  
  
    [Column("rating")]  
    public double Rating { get; set; }  
  
    [Column("description")]  
    public string Description { get; set; }  
  
    [Column("label")]  
    public string Label { get; set; }  
  
    [Column("author_id")]  
    public Guid? AuthorId { get; set; }  
    [Column("BookURL")]  
    public string? BookURL { get; set; }  
}}