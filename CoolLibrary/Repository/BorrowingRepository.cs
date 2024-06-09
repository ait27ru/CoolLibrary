using CoolLibrary.Data;
using CoolLibrary.Models;
using CoolLibrary.Repository.IRepository;
using System.Threading.Tasks;

namespace CoolLibrary.Repository
{
    public class BorrowingRepository : Repository<Borrowing>, IBorrowingRepository
    {
        private readonly ApplicationDbContext _db;

        public BorrowingRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Borrowing borrowing)
        {
            var objFromDb = await FirstOrDefaultAsync(i => i.Id == borrowing.Id);
            if (objFromDb != null)
            {
                objFromDb.BorrowDate = borrowing.BorrowDate;
                objFromDb.ReturnDate = borrowing.ReturnDate;
                objFromDb.ApplicationUserId = borrowing.ApplicationUserId;
                objFromDb.BookId = borrowing.BookId;
            }
        }
    }
}
