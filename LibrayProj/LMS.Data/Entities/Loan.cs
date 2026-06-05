using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Entities;

public class Loan
{
    public int LoanId { get; set; }
    public int BookId { get; set; } // need this? 
    public int BookCopyId { get; set; }
    public BookCopy BookCopy { get; set; }
    public int UserId { get; set; }
    public Book Book { get; set; }   // need this? 
    public User User { get; set; }
    public DateTime LoanedOn { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public bool isReturned => ReturnDate != null;
}