using System;
using System.Collections.Generic;
	
using LMS.Data.Entities;
	
namespace LMS.Data.Services;

// This interface describes the operations that a BookService class should implement
public interface IOverdueChargeService
{
    void Initialise();

    // -------------- OverdueCharge Management -------------------

    Task<OverdueCharge?> CalcChargeForLoanAsync(int loanId);
    Task MarkChargeAsPaid(int chargeId);
    Task<List<OverdueCharge>> GetUnpaidChargesForUserAsync(int userId);
}