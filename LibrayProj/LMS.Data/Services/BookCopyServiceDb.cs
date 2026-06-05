using Microsoft.EntityFrameworkCore;
using LMS.Data.Entities;
using LMS.Data.Repositories;
using System.Runtime.CompilerServices;

namespace LMS.Data.Services;

// EntityFramework Implementation of IBookCopyService
public class BookCopyServiceDb : IBookCopyService
{
    private readonly DatabaseContext db;
    private readonly IBookService bookService;

    public BookCopyServiceDb(DatabaseContext db, IBookService bookService)
    {
        this.db = db;
        this.bookService = bookService;
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }

    // -------------------------- BookCopy Management ---------------------------

    public Task<BookCopy?> GetCopyById(int copyId)
    {
        return db.BookCopies.Include( c => c.Book).FirstOrDefaultAsync( c => c.BookCopyId == copyId);
    }


    public async Task<BookCopy?> AddCopyAsync(int bookId)
    {
        var exist = await bookService.GetBookByIdAsync(bookId);

        if( exist == null) return null;

        var copy = new BookCopy
        {
            IsAvailable = true,
            DateAdded = DateTime.Now
        };

        db.BookCopies.Add(copy);
        await db.SaveChangesAsync();

        return copy;
    }

    
    public async Task<bool> RemoveCopyAsync(int copyId)
    {
        var copy = await GetCopyById(copyId);
        
        if ( copy == null ) return false;

        db.BookCopies.Remove(copy);
        await db.SaveChangesAsync();
        return true;
    }


    public async Task<List<BookCopy>> GetCopiesForBookAsync(int bookId)
    {
        return await db.BookCopies.Where( c => c.BookId == bookId ).OrderBy( c => c.BookCopyId ).ToListAsync();
    }


    public async Task<int> GetAvailableCopiesForBookAsync(int bookId)
    {
        return await db.BookCopies.CountAsync( c => c.BookId == bookId && c.IsAvailable );
    }
}