using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Entities;

    public class Book
    {         
        [Key]
        public int BId { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public DateOnly DateAdded { get; set;}
        public string Availability { get; set; }
        
    
        // suitable recipe attributes / relationships
    }