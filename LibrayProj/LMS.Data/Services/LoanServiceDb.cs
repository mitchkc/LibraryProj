using Microsoft.EntityFrameworkCore;
using LMS.Data.Entities;
using LMS.Data.Repositories;


namespace LMS.Data.Services;

// EntityFramework Implementation of ILoanService
public class LoanServiceDb : ILoanService
{
    private readonly DatabaseContext db;
    private readonly IBookService bookService;

    public LoanServiceDb(DatabaseContext db, IBookService bookService)  // dependency injection 
    {
        this.db = db;
        this.bookService = bookService;
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }

    // ==================== Loan Management ==================
    
    // implement ILoanService methods here

    public async Task<List<Loan>> GetLoansForUserAsync(int userId)
    {
        return await db.Loans.Include( l => l.Book).Where( l => l.UserId == userId).OrderByDescending( l => l.LoanedOn).ToListAsync();
    }


    public async Task<List<Loan>> GetLoansForBookAsync(int bookId)
    {
        return await db.Loans.Include( l => l.User).Where( l => l.BookId == bookId).OrderByDescending( l => l.LoanedOn).ToListAsync();
    }


        public async Task<List<Loan>> GetActiveLoansAsync()
    {
        return await db.Loans.Include( l => l.Book).Include( l => l.User).Where( l => l.ReturnDate == null).ToListAsync();
    }


    public async Task<Loan?> CreateLoanAsync(int bookId, int userId)
    {
        var copy = await db.BookCopies.FirstOrDefaultAsync( c => c.BookId == bookId && c.IsAvailable);

        if( copy == null) return null;

        var loan = new Loan
        {
            BookCopyId = copy.BookCopyId,
            UserId = userId,
            LoanedOn = DateTime.Now,
            DueDate = DateTime.Now.AddDays(14),
            ReturnDate = null
        };

        copy.IsAvailable = false;   // specific copy via its id is no longer available

        db.Loans.Add(loan);
        await db.SaveChangesAsync();
        return loan;
    }

    
    public async Task<Loan?> ReturnBookAsync(int loanId)
    {
        var loan = await db.Loans.Include( l => l.BookCopy).FirstOrDefaultAsync( l => l.LoanId == loanId);

        if(loan == null)   // loan does not exist 
        {
            throw new Exception("Loan not found");
        }

        if(loan.ReturnDate != null)   // loan exists but already returned
        {
            throw new Exception("Book already returned");
        }

        loan.ReturnDate = DateTime.Now;

        loan.BookCopy.IsAvailable = true;

        if(loan.ReturnDate.Value.Date > loan.DueDate.Date)
        {
            var daysLate = (loan.ReturnDate.Value.Date - loan.DueDate.Date).Days;
            var chargeAmount = daysLate * 0.50m;

            var charge = new OverdueCharge
            {
                LoanId = loan.LoanId,
                Amount = chargeAmount,
                CreatedAt = DateTime.Now,
                Ispaid = false
            };

            db.OverdueCharges.Add(charge);
        } 

        await db.SaveChangesAsync();

        return loan;
    }
}