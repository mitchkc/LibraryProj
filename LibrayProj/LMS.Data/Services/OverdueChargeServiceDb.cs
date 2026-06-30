using Microsoft.EntityFrameworkCore;
using LMS.Data.Entities;
using LMS.Data.Repositories;
using System.Collections;


namespace LMS.Data.Services;

// EntityFramework Implementation of IOverdueChargeService
public class OverdueChargeServiceDb : IOverdueChargeService
{
    private readonly DatabaseContext db;
    private readonly IBookService bookService;
    private const decimal DailyCharge = 0.5m; 

    public OverduechargeServiceDb(DatabaseContext db, IBookService bookService)  // dependency injection 
    {
        this.db = db;
        this.bookService = bookService;
    }

    public void Initialise()
    {
        db.Initialise(); // recreate database
    }

    // --------------------- Overdue Charge Management ----------------------------


    public async Task<OverdueCharge?> GetChargeByIdAsync(int chargeId)
    {
        return await db.OverdueCharges.Include( c => c.Loan).ThenInclude( l => l.BookCopy)
            .FirstOrDefaultAsync( c => c.OverdueChargeId == chargeId);
    }

    public async Task<List<OverdueCharge?>> GetUnpaidCharegeForUserAsync(int userId)
    {
        return await db.OverdueCharges.Include( c => c.Loan).ThenInclude( l => l.BookCopy)
            .Where( c => c.Loan.UserId == userId && !c.IsPaid).ToListAsync();
    }

    public async Task<List<OverdueCharge>> GetChargesForLoanAsync(int loanId)
    {
        return await db.OverdueCharges.Where( c => c.LoanId == loanId).ToListAsync();
    }

    public async Task<OverdueCharge?> CreateChargeForLoanAsync(int loandId)
    {
        var loan = await db.Loans.FirstOrDefaultAsync( l => l.LoanId == loanId);

        if(loan == null) return null;

        if(loan.ReturnDate == null) return null;

        if(loan.ReturnDate.Value.Date <= loan.DueDate.Date) return null;

        var daysLate = (loan.ReturnDate.Value.Date - loan.DueDate.Date).Days;

        var amount = daysLate * DailyCharge;

        var charge = new OverdueCharge
        {
            LoandId = loan.LoanId,
            Amount = amount,
            CreatedAt = DateTime.Now,
            IsPaid = false
        };

        db.OverdueCharges.Add(charge);
        await db.SaveChangesAync();

        return charge;
    }

    public async Task <bool> MarkChargeAsPaidAsync(int chargeId)
    {
        var charge = await db.OverdueCharges.FirstOrDefaultAsync( c => c.OverdueChargeId == chargeId);

        if(charge == null) return false;

        if(charge.IsPaid) return true;

        charge.IsPaid = true;
        charge.PaidAt = DateTime.N0w;

        await db.SaveChangesAsync();

        return true;
    }
    
}