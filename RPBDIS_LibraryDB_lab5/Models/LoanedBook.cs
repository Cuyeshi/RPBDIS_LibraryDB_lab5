using System.ComponentModel.DataAnnotations;
using System.Reflection.PortableExecutable;

namespace RPBDIS_LibraryDB_lab5.Models
{
    public class LoanedBook
    {
        [Key]
        public int LoanId { get; set; }

        [Required(ErrorMessage = "Название книги обязательно")]
        public int? BookId { get; set; }

        [Required(ErrorMessage = "Имя читателя обязательно")]
        public int? ReaderId { get; set; }

        [Required(ErrorMessage = "Дата взятия обязательна")]
        public DateOnly LoanDate { get; set; }

        [Required(ErrorMessage = "Дата возвращения обязательна")]
        public DateOnly? ReturnDate { get; set; }

        public bool Returned { get; set; }

        [Required(ErrorMessage = "Имя сотрудника обязательно")]
        public int? EmployeeId { get; set; }

        public virtual Book? Book { get; set; }

        public virtual Employee? Employee { get; set; }

        public virtual Reader? Reader { get; set; }
    }

}
