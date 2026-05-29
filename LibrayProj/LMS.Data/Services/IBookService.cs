using System;
using System.Collections.Generic;
	
using LMS.Data.Entities;
	
namespace LMS.Data.Services;

// This interface describes the operations that a BookService class should implement
public interface IBookService
{
    void Initialise();
        
    // add suitable method definitions to implement assignment requirements      

    // ----------------------- BOOK MANAGEMENT -------------------------
    List<Book> GetBooks(string orderBy="id", string direction="asc"); // other attributes to list by?
    Book GetBook(int id);
    Book AddBook (Book book);
    Book UpdateBook (Book updated);
    bool DeleteBook (int id);
    IList<Book> SearchBooks(string queryName=null, string queryAuthor = null, string queryGenre = null, int? queryPublished = null);

    // ---------------------- REVIEW MANAGEMENT ------------------------      
    Review CreateReview(int bookId, string comment, int rating);
    Review GetReview(int id);
    Review UpdateReview(int id, string comment, int rating);
    bool DeleteReview(int id);
    IList<Review> GetAllReviewsForBook(int bookId);
    Paged<Review> GetReviews(int page=1, int size=10, string orderBy="id", string direction="asc");
}