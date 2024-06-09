using CoolLibrary.Models;
using System.Threading.Tasks;

namespace CoolLibrary.Repository.IRepository
{
    public interface IBorrowingRepository : IRepository<Borrowing>
    {
        Task UpdateAsync(Borrowing borrowing);
    }
}
