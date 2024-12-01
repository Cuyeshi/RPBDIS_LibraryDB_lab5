using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RPBDIS_LibraryDB_lab5.Models;

namespace RPBDIS_LibraryDB_lab5.Data;

public class RPBDIS_LibraryDB_lab5Context : IdentityDbContext<IdentityUser>
{
    public RPBDIS_LibraryDB_lab5Context(DbContextOptions<RPBDIS_LibraryDB_lab5Context> options)
        : base(options)
    {
    }

    
}
