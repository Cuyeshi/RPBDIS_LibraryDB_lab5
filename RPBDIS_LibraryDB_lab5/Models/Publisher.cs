﻿using RPBDIS_LibraryDB_lab5.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPBDIS_LibraryDB_lab5.Models
{
    public partial class Publisher
    {
        [Key]
        public int PublisherId { get; set; }

        [Required(ErrorMessage = "Название издателя обязательно")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Название города обязательно")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "Название адреса обязательно")]
        public string Address { get; set; } = null!;

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}

