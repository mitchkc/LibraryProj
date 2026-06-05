using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Entities;

    public class Book
    {         
        [Key]
        public int BookId { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Synopsis { get; set; }
        public int Published { get; set; }

        // public string IsAvailable { get; set; } - switched to BookCopy, no longer needed
        
        public IList<Review> Reviews { get; set; } = new List<Review>();
        public double AvgRating { get; set; }
        public ICollection<BookCopy> Copies { get; set; }
        public int TotalCopies => Copies.Count;
        public int AVailableCopies => Copies.Count( c => c.IsAvailable);

    }