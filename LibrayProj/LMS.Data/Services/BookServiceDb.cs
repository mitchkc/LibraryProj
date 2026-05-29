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

    public List<Book> GetBooks(string orderBy="id", string direction="asc")
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

        return results.ToList();
    }

    public Paged<Book> GetBooks(int page=1, int pageSize = 10, string orderBy="BookId", string direction="asc")
    {
        var query =(orderBy.ToLower(), direction.ToLower()) switch
        {
            ("bookId", "asc") => db.Books.OrderBy( b => b.BookId),
            ("bookId", "desc") => db.Books.OrderByDescending( b => b.BookId),

            ("name", "asc") => db.Books.OrderBy( b => b.Name),
            ("name", "desc") => db.Books.OrderByDescending( b => b.Name),

            ("genre", "asc") => db.Books.OrderBy( b => b.Genre),
            ("genre", "desc") => db.Books.OrderByDescending( b => b.Genre),

            ("author", "asc") => db.Books.OrderBy( b => b.Author),
            ("author", "desc") => db.Books.OrderByDescending( b => b.Author),

            ("published", "asc") => db.Books.OrderBy( b => b.Published),
            ("published", "desc") => db.Books.OrderByDescending( b => b.Published),

            ("avgRating", "asc") => db.Books.OrderBy( b => b.AvgRating),
            ("avgRating", "desc") => db.Books.OrderByDescending( b => b.AvgRating),
            _ => db.Books.OrderBy( b => b.Name)

        };
        return query.ToPaged(page, pageSize, orderBy, direction);
    }
   
   public Book GetBook(int id)      // get book by id
    {
        return db.Books.Include( m => m.Reviews).FirstOrDefault( b => b.BookId == id);
    }
    public Book GetBookByNameAndAuthor(string name, string author)
    {
        return db.Books.FirstOrDefault( b => b.Name == name && b.Author == author);
    }

    public Book AddBook(Book b)     // add new book to Db
    {
        var exists = GetBookByNameAndAuthor(b.Name, b.Author);
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
        db.SaveChanges();
        return book;        // return book added for display + confirmation
    }

    public bool DeleteBook(int id)
    {
        var del = GetBook(id);
        if(del == null)
        {
            return false;
        }
        db.Books.Remove(del);
        db.SaveChanges();
        return true;
    }

    public Book UpdateBook(Book updated)
    {
        var book = GetBook(updated.BookId);        // verify book exists in Db 1st
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

        db.SaveChanges();
        return book;
    }

    public IList<Book> SearchBooks(string queryName = null, string queryAuthor = null, string queryGenre = null, int? queryPublished = null)
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

        return query.OrderBy( b => b.Name).ToList();
    }


    // --------------------------- REVIEW MANAGEMENT --------------------

    public Review GetReview(int id)
    {
        // returns reviews and assoc movie or null if not found
        return db.Reviews.Include( r => r.Book).FirstOrDefault( r => r.BookId == id );
    }

    public Review CreateReview(int bookId, string comment, int rating)
    {
        var book = GetBook(bookId);
        if (book == null) return null;

        var review = new Review
        {
            BookId = bookId,
            Comment = comment,
            Rating  = rating,
            CreatedOn = DateTime.Now,
        };

        db.Reviews.Add(review);
        db.SaveChanges();
        return review;
    }

    public Review UpdateReview(int id, string comment, int rating)
    {
        var review = GetReview(id);
        if(review == null) return null;

        review.Comment = comment;
        review.Rating = rating;

        db.Reviews.Update(review);
        db.SaveChanges();
        return review;
    }

    public bool DeleteReview(int id)
    {
        var review = GetReview(id);
        if(review == null) return false;

        db.Reviews.Remove(review);
        db.SaveChanges();
        return true; 

    }

    public IList<Review> GetAllReviewsForBook(int bookId)
    {
        var reviews = db.Reviews.Where( r => r.BookId == bookId).ToList();
        return reviews;
    }

    public Paged<Review> GetReviews(int page=1, int pageSize=10, string orderBy="createdOn", string direction="asc")
    {
        var query = (orderBy.ToLower(), direction.ToLower()) switch
        {
            ("revId", "asc") => db.Reviews.OrderBy( r => r.RevId),
            ("revId", "desc") => db.Reviews.OrderByDescending( r => r.RevId),
            ("createdOn", "asc") => db.Reviews.OrderBy( r => r.CreatedOn),
            ("createdOn", "desc") => db.Reviews.OrderByDescending( r => r.CreatedOn),
            ("rating", "asc") => db.Reviews.OrderBy( r => r.Rating),
            ("rating", "desc") => db.Reviews.OrderByDescending( r => r.Rating),
            _ => db.Reviews.OrderBy( r => r.CreatedOn)
        };
        return query.ToPaged(page, pageSize, orderBy, direction);
    }

}