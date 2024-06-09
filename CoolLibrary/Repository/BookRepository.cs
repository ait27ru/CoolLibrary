using CoolLibrary.Data;
using CoolLibrary.Models;
using CoolLibrary.Repository.IRepository;
using System.Threading.Tasks;

namespace CoolLibrary.Repository
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        private readonly ApplicationDbContext _db;

        public BookRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Book book)
        {
            var bookFromDb = await FirstOrDefaultAsync(i => i.Id == book.Id);

            if (bookFromDb != null)
            {
                bookFromDb.Title = book.Title;
                bookFromDb.Description = book.Description;
                bookFromDb.Author = book.Author;
                bookFromDb.PublishedYear = book.PublishedYear;
                bookFromDb.Description = book.Description;
                bookFromDb.Quantity = book.Quantity;
                bookFromDb.GenreId = book.GenreId;
            }
        }
    }
}
