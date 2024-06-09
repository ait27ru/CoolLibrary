using CoolLibrary.Models;
using System.Threading.Tasks;

namespace CoolLibrary.Repository.IRepository
{
    public interface IGenreRepository : IRepository<Genre>
    {
        Task UpdateAsync(Genre genre);
    }
}
