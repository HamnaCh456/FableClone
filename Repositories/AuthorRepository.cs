using MyMvcAuthProject.Models;
using Supabase;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMvcAuthProject.Repositories
{
    public class AuthorRepository
    {
        private readonly Client _supabase;

        public AuthorRepository(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<List<Author>> GetAuthorByAuthorId(Guid authorId)
        {
            var result = await _supabase
                .From<Author>()
                .Select("author_id, name, bio, author_image")
                .Where(x => x.AuthorId == authorId)
                .Get();

            return result.Models;
        }

        public async Task<Author> GetAuthorByBookId(Guid bookId)  
        {  
            // First get the book to find the author_id  
            var book = await _supabase  
                .From<Book>()  
                .Select("author_id")  
                .Where(x => x.BookId == bookId)  
                .Single();  
            
            // Then get the author details  
            var author = await _supabase  
                .From<Author>()  
                .Select("author_id, name, bio, author_image")
                .Where(x => x.AuthorId == book.AuthorId)  
                .Single();  
            
            return author;  
        }
    }
}
