using Microsoft.EntityFrameworkCore;
using LMS.Data.Entities;
using LMS.Data.Repository;

namespace LMS.Data.Services;

// EntityFramework Implementation of IBookService
public class BookServiceDb : IBookService
{
    private readonly DataContext db;

    public BookServiceDb()
    {
        db = new DataContext();
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }

    // ==================== Book Management ==================
    
    // implement IBookService methods here
   


}