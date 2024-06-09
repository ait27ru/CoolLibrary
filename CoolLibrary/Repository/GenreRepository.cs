using CoolLibrary.Data;
using CoolLibrary.Models;
using CoolLibrary.Repository.IRepository;
using System.Threading.Tasks;

namespace CoolLibrary.Repository
{
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        private readonly ApplicationDbContext _db;

        public GenreRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task UpdateAsync(Genre genre)
        {
            var objFromDb = await FirstOrDefaultAsync(i => i.Id == genre.Id);
            if (objFromDb != null)
            {
                objFromDb.DisplayOrder = genre.DisplayOrder;
                objFromDb.Name = genre.Name;
            }
        }
    }
}
