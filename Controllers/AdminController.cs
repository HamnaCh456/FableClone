using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MyMvcAuthProject.Hubs;
using MyMvcAuthProject.Models;
using MyMvcAuthProject.Models.ViewModels;
using MyMvcAuthProject.Repositories;

namespace MyMvcAuthProject.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly Supabase.Client _supabase;
        private readonly IHubContext<NotificationHub> _hubContext;

        public AdminController(Supabase.Client supabase, IHubContext<NotificationHub> hubContext)
        {
            _supabase = supabase;
            _hubContext = hubContext;
        }

        // GET: Admin/AddBook
        public IActionResult AddBook()
        {
            return View();
        }

        // POST: Admin/AddBook
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBook(AdminAddBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Create author repository
                var authorRepository = new AuthorRepository(_supabase);
                
                // Create and insert author
                var author = new Author
                {
                    Name = model.AuthorName,
                    Bio = model.AuthorBio,
                    AuthorImage = model.AuthorImage
                };

                var insertedAuthor = await authorRepository.InsertAuthor(author);

                if (insertedAuthor == null)
                {
                    ModelState.AddModelError("", "Failed to add author.");
                    return View(model);
                }

                // Create book repository
                var bookRepository = new BookRepository(_supabase);

                // Create and insert book
                var book = new Book
                {
                    Title = model.Title,
                    Price = model.Price,
                    Rating = model.Rating,
                    Description = model.Description,
                    Label = model.Label,
                    BookURL = model.BookURL,
                    AuthorId = insertedAuthor.AuthorId
                };

                var insertedBook = await bookRepository.InsertBook(book);

                if (insertedBook == null)
                {
                    ModelState.AddModelError("", "Failed to add book.");
                    return View(model);
                }

                // Send notification to all connected clients
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", 
                    insertedBook.Title, 
                    insertedAuthor.Name);

                TempData["SuccessMessage"] = $"Book '{model.Title}' by {model.AuthorName} added successfully!";
                return RedirectToAction("AddBook");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(model);
            }
        }
    }
}
