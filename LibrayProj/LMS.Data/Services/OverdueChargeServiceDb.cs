using Microsoft.EntityFrameworkCore;
using LMS.Data.Entities;
using LMS.Data.Repositories;


namespace LMS.Data.Services;

// EntityFramework Implementation of IOverdueChargeService
public class OverdueChargeServiceDb : IOverdueChargeService
{
    private readonly DatabaseContext db;
    private readonly IBookService bookService;

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


    public async Task<OverdueCharge?> CalcChargeForLoanAsync(int loanId)
    {
        
    }
    public Task MarkChargeAsPaid(int chargeId)
    {
        
    }
    public async Task<List<OverdueCharge>> GetUnpaidChargesForUserAsync(int userId)
    {
        
    }
}