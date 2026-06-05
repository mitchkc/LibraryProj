using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Entities;

public class OverdueCharge
{
    public int OverdueChargeId { get; set; }
    public int LoanId { get; set; }
    public Loan Loan { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
}