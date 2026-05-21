using System.ComponentModel.DataAnnotations;
using System.Dynamic;

namespace LMS.Data.Entities;
public class Review
{     
    [Key]
    public int RId { get; set; }
    public int BookId { get; set; } // foreign key
    public int Userid { get; set; }
    public Book Book { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.MinValue; //default 
    public string Comment { get; set; }
    public int Rating { get; set; }

    
    // suitable recipe attributes / relationships


}