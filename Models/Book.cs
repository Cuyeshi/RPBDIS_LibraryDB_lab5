using System.ComponentModel.DataAnnotations;
namespace RPBDIS_LibraryDB_lab5.Models

{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public int? PublisherId { get; set; }

        public int PublishYear { get; set; }

        public int? GenreId { get; set; }

        public decimal Price { get; set; }

        public virtual Genre? Genre { get; set; }

        public virtual ICollection<LoanedBook> LoanedBooks { get; set; } = new List<LoanedBook>();

        public virtual Publisher? Publisher { get; set; }
    }

}
