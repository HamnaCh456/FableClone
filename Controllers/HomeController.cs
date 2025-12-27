using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MyMvcAuthProject.Models;
using MyMvcAuthProject.Repositories;

namespace MyMvcAuthProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly BookRepository _bookRepository;
    private readonly AuthorRepository _authorRepository;
    private readonly PostRepository _postRepository;

    public HomeController(
        ILogger<HomeController> logger,
        BookRepository bookRepository,
        AuthorRepository authorRepository,
        PostRepository postRepository)
    {
        _logger = logger;
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _postRepository = postRepository;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            // Fetch 5 books from the database
            var allBooks = await _bookRepository.GetAllBooks(); // Get all books
            var books = allBooks.Take(5).ToList();

            // Fetch recent posts
            var posts = await _postRepository.GetAllPostsAsync();
            var recentPosts = posts.OrderByDescending(p => p.PostId).Take(2).ToList();

            // Fetch a featured author (first author from the database)
            Author featuredAuthor = null;
            if (books.Any() && books.First().AuthorId.HasValue)
            {
                var authorList = await _authorRepository.GetAuthorByAuthorId(books.First().AuthorId.Value);
                featuredAuthor = authorList.FirstOrDefault();
            }

            // Pass data to view using ViewBag
            ViewBag.Books = books;
            ViewBag.Posts = recentPosts;
            ViewBag.FeaturedAuthor = featuredAuthor;
            ViewBag.FeaturedBook = books.FirstOrDefault();

            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page data");
            // Return view with empty data
            ViewBag.Books = new List<Book>();
            ViewBag.Posts = new List<Post>();
            ViewBag.FeaturedAuthor = null;
            ViewBag.FeaturedBook = null;
            return View();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
