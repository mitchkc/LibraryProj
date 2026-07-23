using System.ComponentModel.Design;
using System.Drawing;
using System.Security.AccessControl;
using System.Transactions;
using LMS.Data.Entities;
using LMS.Data.Services;
using Microsoft.AspNetCore.MVC;

namespace LMS.Web.Controllers;

public class BookController : BaseController
{
    private IBookService svc;

    private readonly ILogger<HomeController> logger;

    public BookController(IBookService _svc, ILogger<HomeController> _logger)
    {
        svc = _svc;
        logger = _logger;
    }


    // index to list all books
    public async Task<IActionResult> Index( int page = 1, int pageSize = 10, string orderBy = "Name", string direction = "asc")
    {
        var books = await ServiceContainer.GetBooksAsync( page, pageSize, orderBy, direction);

        return View(books);
    }

    public async Task<IAcionResult> Deatils(int id)
    {
        var book = await ServiceContainer.GetBookByIdAsync(id);

        if( book == null) return NotFound();

        return View(book);
    }

    // GET create new book
    [Authorize(Roles="admin,staff")]
    public IActionResult Create()
    {
        return View();
    }

    // POST book add new
    [HttpPost]
    [ValidateAntiforgeryToken]
    [Authorize(Roles)]
    public async Task<IActionResult> Create(Book book)
    {
        if(!ModelState.IsValid) return View(book);

        await Service.AddBookAsync(book);

        return RedirectToAction(nameof(Index));
    }
    
}