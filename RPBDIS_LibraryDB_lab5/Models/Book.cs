using System.ComponentModel.DataAnnotations;
namespace RPBDIS_LibraryDB_lab5.Models

{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Название книги обязательно")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Автор книги обязателен")]
        public string Author { get; set; } = null!;

        [Required(ErrorMessage = "Выберите издателя")]
        public int? PublisherId { get; set; }

        [Required(ErrorMessage = "Год издания обязателен")]
        [Range(1001, 3000, ErrorMessage = "Год издания должен быть больше 1000 и меньше 3000")]
        public int PublishYear { get; set; }

        [Required(ErrorMessage = "Выберите жанр")]
        public int? GenreId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Цена не может быть отрицательной")]
        public decimal Price { get; set; }

        public virtual Genre? Genre { get; set; }

        public virtual ICollection<LoanedBook> LoanedBooks { get; set; } = new List<LoanedBook>();

        
        public virtual Publisher? Publisher { get; set; }
    }

}
