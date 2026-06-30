using System;
using System.Collections.Generic;
	
using LMS.Data.Entities;
	
namespace LMS.Data.Services;

// This interface describes the operations that a OverdueChargeService class should implement
public interface IOverdueChargeService
{
    void Initialise();

    // -------------- OverdueCharge Management -------------------

    Task<OverdueCharge?> GetChargeByIdAsync(int chargeId);
    Task<List<OverdueCharge>> GetUnpaidChargesForUserAsync(int userId);
    Task<List<OverdueCharge>> GetChargesForLoanAsync(int loanId);
    Task<List<OverdueCharge?>> CreateChargeForLoanAsync(int loanId);
    Task<bool> MarkChargeAsPaidAsync(int chargeId);
    
}