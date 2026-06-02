using Microsoft.EntityFrameworkCore;
using LMS.Data.Entities;
using LMS.Data.Repositories;

namespace LMS.Data.Services;

// EntityFramework Implementation of IBookService
public class BookServiceDb : IBookService
{
    private readonly DatabaseContext db;

    public BookServiceDb(DatabaseContext db)
    {
        this.db = db;
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }

    // ==================== Book Management ==================
    
    // implement IBookService methods here

    public async Task<List<Book>> GetBooksAsync(string orderBy="id", string direction="asc")
    {
        var results = (orderBy.ToLower(), direction.ToLower()) switch
        {
            ("name","asc") => db.Books.OrderBy(r => r.Name),
            ("name","desc") => db.Books.OrderByDescending(r => r.Name),

            ("author","asc") => db.Books.OrderBy(r => r.Author),
            ("author","desc") => db.Books.OrderByDescending(r => r.Author),

            ("genre","asc") => db.Books.OrderBy( r => r.Genre),
            ("genre","desc") => db.Books.OrderByDescending( r => r.Genre),

            ("availability","desc") => db.Books.OrderByDescending(r => r.Availability),
            ("availability","asc") => db.Books.OrderBy(r => r.Availability),

            ("published","asc") => db.Books.OrderBy(r => r.Published),
            ("published","desc") => db.Books.OrderByDescending(r => r.Published),
            _ => db.Books.OrderBy(r => r.Name)
        };

        return await results.ToListAsync();
    }

    public async Task<Paged<Book>> GetBooksAsync(int page=1, int pageSize = 10, string orderBy="BookId", string direction="asc")
    {
        var query =(orderBy.ToLower(), direction.ToLower()) switch
        {
            ("bookid", "asc") => db.Books.OrderBy( b => b.BookId),
            ("bookid", "desc") => db.Books.OrderByDescending( b => b.BookId),

            ("name", "asc") => db.Books.OrderBy( b => b.Name),
            ("name", "desc") => db.Books.OrderByDescending( b => b.Name),

            ("genre", "asc") => db.Books.OrderBy( b => b.Genre),
            ("genre", "desc") => db.Books.OrderByDescending( b => b.Genre),

            ("author", "asc") => db.Books.OrderBy( b => b.Author),
            ("author", "desc") => db.Books.OrderByDescending( b => b.Author),

            ("published", "asc") => db.Books.OrderBy( b => b.Published),
            ("published", "desc") => db.Books.OrderByDescending( b => b.Published),

            ("avgrating", "asc") => db.Books.OrderBy( b => b.AvgRating),
            ("avgrating", "desc") => db.Books.OrderByDescending( b => b.AvgRating),
            _ => db.Books.OrderBy( b => b.Name)

        };
        return await query.ToPaged(page, pageSize, orderBy, direction);
    }
   
   public Task<Book?> GetBookByIdAsync(int id)      // get book by id
    {
        return db.Books
            .Include( m => m.Reviews)
            .FirstOrDefaultAsync( b => b.BookId == id);
    }
    public Task<Book?> GetBookByNameAndAuthorAsync(string name, string author)
    {
        return db.Books
        .FirstOrDefaultAsync( b => b.Name == name && b.Author == author);
    }

    public async Task<Book?> AddBookAsync(Book b)     // add new book to Db
    {
        var exists = await GetBookByNameAndAuthorAsync(b.Name, b.Author);
        if(exists != null)
        {
            return null;
        }
        var book = new Book
        {
            Name = b.Name,
            Author = b.Author,
            Genre = b.Genre,
            Synopsis = b.Synopsis,
            Published = b.Published,
            DateAdded = b.DateAdded,
            Availability = b.Availability,
        };
        db.Books.Add(book);
        await db.SaveChangesAsync();
        return book;        // return book added for display + confirmation
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var del = await GetBookByIdAsync(id);
        if(del == null)
        {
            return false;
        }
        db.Books.Remove(del);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<Book?> UpdateBookAsync(Book updated)
    {
        var book = await GetBookByIdAsync(updated.BookId);        // verify book exists in Db 1st
        if(book == null)
        {
            return null;
        }
        book.Name = updated.Name;
        book.Author = updated.Author;
        book.Genre = updated.Genre;
        book.Synopsis= updated.Synopsis;
        book.Published = updated.Published;
        book.DateAdded = updated.DateAdded;

        await db.SaveChangesAsync();
        return book;
    }

    public async Task<IList<Book>> SearchBooksAsync(
        string? queryName = null, string? queryAuthor = null,
        string? queryGenre = null, int? queryPublished = null)
    {
        queryName = queryName?.ToLower();
        queryAuthor = queryAuthor?.ToLower();
        queryGenre = queryGenre?.ToLower();

        var query = db.Books.AsQueryable();

        if(!string.IsNullOrEmpty(queryName))
        {
            query = query.Where( b => b.Name.ToLower().Contains(queryName));
        }
        if(!string.IsNullOrEmpty(queryAuthor))
        {
            query = query.Where( b => b.Author.ToLower().Contains(queryAuthor));
        }
        if(!string.IsNullOrEmpty(queryGenre))
        {
            query = query.Where( b => b.Genre.ToLower().Contains(queryGenre));
        }
        if(queryPublished.HasValue)
        {
            query = query.Where( b => b.Published == queryPublished.Value);
        }

        return await query.OrderBy( b => b.Name).ToListAsync();
    }


    // --------------------------- REVIEW MANAGEMENT --------------------

    public Task<Review?> GetReviewByIdAsync(int id)
    {
        // returns reviews and assoc movie or null if not found
        return db.Reviews.Include( r => r.Book).FirstOrDefaultAsync( r => r.BookId == id );
    }

    public async Task<Review?> CreateReviewAsync(int bookId, string comment, int rating)
    {
        var book = await GetBookByIdAsync(bookId);
        if (book == null) return null;

        var review = new Review
        {
            BookId = bookId,
            Comment = comment,
            Rating  = rating,
            CreatedOn = DateTime.Now,
        };

        db.Reviews.Add(review);
        await db.SaveChangesAsync();
        return review;
    }

    public async Task<Review?> UpdateReviewAsync(int id, string comment, int rating)
    {
        var review = await GetReviewByIdAsync(id);
        if(review == null) return null;

        review.Comment = comment;
        review.Rating = rating;

        db.Reviews.Update(review);
        await db.SaveChangesAsync();
        return review;
    }

    public async Task<bool> DeleteReviewAsync(int id)
    {
        var review = await GetReviewByIdAsync(id);
        if(review == null) return false;

        db.Reviews.Remove(review);
        await db.SaveChangesAsync();
        return true; 

    }

    public async Task<IList<Review>> GetAllReviewsForBookAsync(int bookId)
    {
        var reviews = await db.Reviews.Where( r => r.BookId == bookId).ToListAsync();
        return reviews;
    }

    public async Task<Paged<Review>> GetReviewsAsync(int page=1, int pageSize=10, string orderBy="createdOn", string direction="asc")
    {
        var query = (orderBy.ToLower(), direction.ToLower()) switch
        {
            ("revid", "asc") => db.Reviews.OrderBy( r => r.RevId),
            ("revid", "desc") => db.Reviews.OrderByDescending( r => r.RevId),
            ("createdon", "asc") => db.Reviews.OrderBy( r => r.CreatedOn),
            ("createdon", "desc") => db.Reviews.OrderByDescending( r => r.CreatedOn),
            ("rating", "asc") => db.Reviews.OrderBy( r => r.Rating),
            ("rating", "desc") => db.Reviews.OrderByDescending( r => r.Rating),
            _ => db.Reviews.OrderBy( r => r.CreatedOn)
        };
        return await query.ToPaged(page, pageSize, orderBy, direction);
    }

}