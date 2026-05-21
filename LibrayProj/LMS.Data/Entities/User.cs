using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Entities;

public enum Role { admin, staff, member }

public class User {
    [Key]
    public int UId { get; set; }
    public string Forename { get; set; }
    public string Surname { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
    public DateOnly DateOfRegistration { get; set;}
    public string Email { get; set; }
    public string Address { get; set; }
    public string Gender { get; set; }
    
    [DisplayFormat(DataFormatString = "{0:yyy:mm-dd}", ApplyFormatInEditMode = true)]
    [DataType(DataType.Date)]
    public DateTime DoB { get; set; }
    public int Age => (int)(DateTime.Now - DoB).TotalDays / 365;
    public string ContactNumber { get; set; }
}