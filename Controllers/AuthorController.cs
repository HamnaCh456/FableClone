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

        public AuthorController(Client supabase)
        {
            _supabase = supabase;
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

        [HttpGet("Author/Search")]
        public IActionResult Search()
        {
            return View("SearchAuthors");
        }

        [HttpGet("Author/Store")]
        public async Task<IActionResult> Store(string name)
        {
             // Re-initialize if supabase was not properly handled or just reuse _supabase if it's already open
             // The constructor keeps it open.
            
            var authorRepository = new AuthorRepository(_supabase);
            var authors = await authorRepository.SearchAuthorsByName(name);
            return PartialView("_AuthorSearchResults", authors);
        }
    }
}
