using CoolLibrary.Data;
using CoolLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CoolLibrary.Initialiser
{
    public class DbInitialiser : IDbInitialiser
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitialiser(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialise()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { }

            if (!_roleManager.RoleExistsAsync(AppGlobals.AdminRole).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(AppGlobals.AdminRole)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(AppGlobals.CustomerRole)).GetAwaiter().GetResult();
            }

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                FullName = "Admin",
                PhoneNumber = "1234567890"
            }, "Test1234$").GetAwaiter().GetResult();

            var admin = _db.ApplicationUser.FirstOrDefault(u => u.Email == "admin@gmail.com");
            _userManager.AddToRoleAsync(admin, AppGlobals.AdminRole).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "joe.bloggs@gmail.com",
                Email = "joe.bloggs@gmail.com",
                EmailConfirmed = true,
                FullName = "Joe Bloggs",
                PhoneNumber = "1234567890"
            }, "Test1234$").GetAwaiter().GetResult();

            var user1 = _db.ApplicationUser.FirstOrDefault(u => u.Email == "joe.bloggs@gmail.com");
            _userManager.AddToRoleAsync(user1, AppGlobals.CustomerRole).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "john.smith@gmail.com",
                Email = "john.smith@gmail.com",
                EmailConfirmed = true,
                FullName = "John Smith",
                PhoneNumber = "1234567890"
            }, "Test1234$").GetAwaiter().GetResult();

            var user2 = _db.ApplicationUser.FirstOrDefault(u => u.Email == "john.smith@gmail.com");
            _userManager.AddToRoleAsync(user2, AppGlobals.CustomerRole).GetAwaiter().GetResult();

            var genreExists = _db.Genre.Any();

            if (!_db.Genre.Any())
            {
                _db.Genre.Add(new Genre
                {
                    DisplayOrder = 1,
                    Name = "Science Fiction"
                });
                _db.Genre.Add(new Genre
                {
                    DisplayOrder = 2,
                    Name = "Fantasy"
                });
                _db.Genre.Add(new Genre
                {
                    DisplayOrder = 3,
                    Name = "Horror"
                });

                _db.SaveChanges();
            }

            if (!_db.Book.Any())
            {
                _db.Book.Add(new Book
                {
                    Title = "Dune",
                    Author = "Frank Herbert",
                    PublishedYear = 2010,
                    Description = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</p>",
                    GenreId = 1,
                    Quantity = 15
                });
                _db.Book.Add(new Book
                {
                    Title = "Black Magician",
                    Author = "Trudi Canavan",
                    PublishedYear = 2010,
                    Description = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</p>",
                    GenreId = 2,
                    Quantity = 10
                });
                _db.Book.Add(new Book
                {
                    Title = "The Shining",
                    Author = "Stephen King",
                    PublishedYear = 1977,
                    Description = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.</p>",
                    GenreId = 3,
                    Quantity = 25
                });

                _db.SaveChanges();
            }
        }
    }
}
