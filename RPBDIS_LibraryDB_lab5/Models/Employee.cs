using RPBDIS_LibraryDB_lab5.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPBDIS_LibraryDB_lab5.Models
{
    public partial class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Полное имя обязательно")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Должность обязательна")]
        public string? Position { get; set; }

        [Required(ErrorMessage = "Дата найма обязательна")]
        public DateOnly? HireDate { get; set; }

        public virtual ICollection<LoanedBook> LoanedBooks { get; set; } = new List<LoanedBook>();
    }

}

