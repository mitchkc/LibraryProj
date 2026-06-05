using System;
using System.Collections.Generic;
	
using LMS.Data.Entities;
	
namespace LMS.Data.Services;

// This interface describes the operations that a BookCopyService class should implement
public interface IBookCopyService
{
    void Initialise();    

    // ----------------------- BookCopy Management -------------------------

    Task<BookCopy?> AddCopyAsync(int bookId);
    Task<bool> RemoveCopyAsync(int copyId);
    Task<List<BookCopy>> GetCopiesForBookAsync(int bookId);
    Task<int> GetAvailableCopiesForBookAsync(int bookId);

}