using System.ComponentModel.DataAnnotations;

namespace LMS.Data.Entities;

    public class BookCopy
    {         
        [Key]
        public int BookCopyId { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime DateAdded { get; set;}

    }