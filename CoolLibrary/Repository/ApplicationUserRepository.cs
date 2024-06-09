using CoolLibrary.Data;
using CoolLibrary.Models;
using CoolLibrary.Repository.IRepository;

namespace CoolLibrary.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
