using Microsoft.EntityFrameworkCore;
using MMS.Data.Entities;
using MMS.Data.Repository;

namespace MMS.Data.Services;

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