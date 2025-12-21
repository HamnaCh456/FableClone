using MyMvcAuthProject.Models;  
using Supabase;  
using Supabase.Postgrest;  
  
namespace MyMvcAuthProject.Repositories  
{  
    public class BookRepository  
    {  
        private readonly Supabase.Client _supabase;  
  
        public BookRepository(Supabase.Client supabase)  
        {  
            _supabase = supabase;  
        }  
  
        public async Task<List<Book>> GetBookById(string bookId = "4c703670-360f-4ade-8644-37de2200993e")  
        {  
            var guid = Guid.Parse(bookId);  
            var result = await _supabase  
                .From<Book>()  
                .Select("book_id, title, rating, description, label, author_id, BookURL")  
                .Where(x => x.BookId == guid)  
                .Get();  
            return result.Models;  
        }  
      
        public async Task<List<Book>> GetBooksByAuthorId(Guid authorId)  
        {  
            var result = await _supabase  
                .From<Book>()  
                .Select("book_id, title, price, rating, description, label, author_id, BookURL")  
                .Where(x => x.AuthorId == authorId)  
                .Get();  
  
            return result.Models;  
        }  
  
       public async Task<List<Book>> SearchBooksByName(string bookName)  
        {  
            var result = await _supabase  
                .From<Book>()  
                .Select("book_id, title, price, rating, description, label, author_id, BookURL")  
                .Filter(x => x.Title, Supabase.Postgrest.Constants.Operator.ILike, $"%{bookName}%")  
                .Get();  
            
            return result.Models;  
        }
    }  
}