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
    Task<List<Book>> GetBooksAsync(string orderBy="id", string direction="asc"); // other attributes to list by?
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book?> AddBookAsync (Book book);
    Task<Book?> UpdateBookAsync (Book updated);
    Task<bool> DeleteBookAsync (int id);
    Task<IList<Book>> SearchBooksAsync(string? queryName=null, string? queryAuthor = null, string? queryGenre = null, int? queryPublished = null);

    // ---------------------- REVIEW MANAGEMENT ------------------------      
    Task<Review?> CreateReviewAsync(int bookId, string comment, int rating);
    Task<Review?> GetReviewByIdAsync(int id);
    Task<Review?> UpdateReviewAsync(int id, string comment, int rating);
    Task<bool> DeleteReviewAsync(int id);
    Task<IList<Review>> GetAllReviewsForBookAsync(int bookId);
    Task<Paged<Review>> GetReviewsAsync(int page=1, int size=10, string orderBy="id", string direction="asc");
}