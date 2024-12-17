using System.ComponentModel.DataAnnotations;

namespace RPBDIS_LibraryDB_lab5.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Название жанра обязательно")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Описание обязательно")]
        public string? Description { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }

}
