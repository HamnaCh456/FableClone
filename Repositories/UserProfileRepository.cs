using Microsoft.EntityFrameworkCore;
using MyMvcAuthProject.Data;
using MyMvcAuthProject.Models;
using System.Threading.Tasks;
using Supabase;

namespace MyMvcAuthProject.Repositories
{
    public class UserProfileRepository
    {
        private readonly Client _supabase;

        public UserProfileRepository(Client supabase)
        {
            _supabase = supabase;
        }

        public async Task<UserProfile?> GetUserByUserId(string userId)  
        {  
            var result = await _supabase  
               .From<UserProfile>()  
               .Where(x => x.UserId == userId)  
               .Get();  
               
            return result.Models.FirstOrDefault();  
        }
        public async Task<UserProfile?> SaveUserData(UserProfile userProfile)  
        {  
            var result = await _supabase  
                .From<UserProfile>()  
                .Upsert(userProfile);  
                
            return result.Models.FirstOrDefault();  
        }
        public async Task<ListBooks> AddBookToListAsync(string userId, Guid bookId, string listTag)
        {
            // Validate list tag
            var validTags = new[] { "Want to Read", "Finished", "Currently Reading", "Did Not Finish" };
            if (!validTags.Contains(listTag))
                throw new ArgumentException("Invalid list tag", nameof(listTag));

            // Find or create the list for this user and tag
            var list = await GetOrCreateListAsync(userId, listTag);

            // Add book to list
            var listBook = new ListBooks
            {
                ListId = list.ListId,
                BookId = bookId
            };

            var response = await _supabase.From<ListBooks>().Insert(listBook);
            return response.Models.FirstOrDefault() ?? listBook;
        }

        public async Task<ReadingList> GetOrCreateListAsync(string userId, string listName)
        {
            // Try to find existing list
            var existingList = await _supabase
                .From<ReadingList>()
                .Where(x => x.UserId == userId && x.ListName == listName)
                .Get();

            var list = existingList.Models.FirstOrDefault();

            if (list != null)
                return list;

            // Create new list if not found
            var newList = new ReadingList
            {
                ListId = Guid.NewGuid(),
                UserId = userId,
                ListName = listName
            };

            var response = await _supabase.From<ReadingList>().Insert(newList);
            return response.Models.First();
        }

        public async Task<Dictionary<string, List<Guid>>> GetUserListsAsync(string userId)
        {
            var userLists = new Dictionary<string, List<Guid>>();
            var validTags = new[] { "Want to Read", "Finished", "Currently Reading", "Did Not Finish" };

            // Fetch all lists for the user
            var listsResponse = await _supabase
                .From<ReadingList>()
                .Where(x => x.UserId == userId)
                .Get();

            foreach (var tag in validTags)
            {
                userLists[tag] = new List<Guid>();
                
                var list = listsResponse.Models.FirstOrDefault(l => l.ListName == tag);
                if (list != null)
                {
                    // For each list, fetch the book IDs
                    // Note: Supabase-csharp join support might be limited, so doing n+1 query for simplicity now 
                    // or ideally we key on ListId.
                    
                    var booksResponse = await _supabase
                        .From<ListBooks>()
                        .Where(x => x.ListId == list.ListId)
                        .Get();
                        
                    userLists[tag] = booksResponse.Models.Select(lb => lb.BookId).ToList();
                }
            }

            return userLists;
        }  
       
    }
}
