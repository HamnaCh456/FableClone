using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Models;
using MyMvcAuthProject.Data;
using Supabase;
using MyMvcAuthProject.Repositories;
using MyMvcAuthProject.ViewModels;

namespace MyMvcAuthProject.Controllers
{
    public class AuthorController : Controller
    {
        private readonly Client _supabase;

        public AuthorController()
        {
            var url = "https://phqjkkhovqndiyyuwljc.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBocWpra2hvdnFuZGl5eXV3bGpjIiwicm9sZSI6InNlcnZpY2Vfcm9sZSIsImlhdCI6MTc2MzExNDc0MywiZXhwIjoyMDc4NjkwNzQzfQ.ZPEqacRXPHk1FdJPMfbGohMyTW0oIpnxuPrzQePlLVI";

            var options = new SupabaseOptions
            {
                AutoConnectRealtime = true
            };

            _supabase = new Client(url, key, options);
            _supabase.InitializeAsync().Wait();
        }

        public async Task<IActionResult> AuthorProfile(Guid authorId)
        {
            // Initialize repositories
            var authorRepository = new AuthorRepository(_supabase);
            var bookRepository = new BookRepository(_supabase);

            // Get author details
            var authors = await authorRepository.GetAuthorByAuthorId(authorId);
            if (authors == null || authors.Count == 0)
            {
                return NotFound("Author not found");
            }

            var author = authors[0];
            ViewBag.Author = author;

            // Get books by this author
            var books = await bookRepository.GetBooksByAuthorId(authorId);

            return View("AuthorProfile", books);
        }
    }
}
