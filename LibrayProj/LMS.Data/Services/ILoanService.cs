using System;
using System.Collections.Generic;
	
using LMS.Data.Entities;
	
namespace LMS.Data.Services;

// This interface describes the operations that the LoanDb Service class should implement
public interface ILoanService
{
    void Initialise();

    Task<List<Loan>> GetLoansForUserAsync(int userId);
    Task<List<Loan>> GetLoansForBookAsync(int bookId);
    Task<List<Loan>> GetActiveLoansAsync();
    Task<Loan?> CreateLoanAsync(int bookId, int userId);
    Task<Loan?> ReturnBookAsync(int loadId);

}