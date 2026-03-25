using Core.Abstracts.IRepositories;
using Core.Concretes.Entities;
using Utils.Generics;

namespace Data.Repositories
{
    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(ShopContext db) : base(db)
        {
        }
    }
}