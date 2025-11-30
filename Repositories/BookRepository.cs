using MyMvcAuthProject.Models;  
using Supabase;  
  
namespace MyMvcAuthProject.Repositories  
{  
    public class BookRepository  
    {  
        private readonly Client _supabase;  
  
        public BookRepository(Client supabase)  
        {  
            _supabase = supabase;  
        }  
  
        public async Task<List<Book>> GetAllBooks()  
        {  
           var bookId = Guid.Parse("4c703670-360f-4ade-8644-37de2200993e");  
           var result = await _supabase  
                              .From<Book>()  
                              .Select("book_id, title, rating, description, label, author_id, BookURL")  
                              .Where(x => x.BookId == bookId)  
                              .Get();
            return result.Models;  
        }  
    }  
}