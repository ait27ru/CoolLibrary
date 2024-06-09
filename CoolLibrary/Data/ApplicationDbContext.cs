using CoolLibrary.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoolLibrary.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public ApplicationDbContext() { }
        public DbSet<Genre> Genre { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Borrowing> Borrowing { get; set; }
    }
}
