using CoolLibrary.Models;
using System.Threading.Tasks;

namespace CoolLibrary.Repository.IRepository
{
    public interface IBookRepository : IRepository<Book>
    {
        Task UpdateAsync(Book book);
    }
}
