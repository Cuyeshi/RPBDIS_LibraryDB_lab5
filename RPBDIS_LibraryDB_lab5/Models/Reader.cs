using RPBDIS_LibraryDB_lab5.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPBDIS_LibraryDB_lab5.Models;

public partial class Reader
{
    [Key]
    public int ReaderId { get; set; }

    [Required(ErrorMessage = "Полное имя обязательно")]
    public string FullName { get; set; } = null!;

    [Required(ErrorMessage = "Дата рождения обязательна")]
    public DateOnly BirthDate { get; set; }

    public string Gender { get; set; } = null!;

    [Required(ErrorMessage = "Название адреса обязательно")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "Номер телефона обязателен")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Паспортные данные обязательны")]
    public string Passport { get; set; } = null!;

    public virtual ICollection<LoanedBook> LoanedBooks { get; set; } = new List<LoanedBook>();
}
