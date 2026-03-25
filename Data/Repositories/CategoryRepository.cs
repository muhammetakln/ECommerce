using Core.Abstracts.IRepositories;
using Core.Concretes.Entities;
using Utils.Generics;

namespace Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ShopContext db) : base(db)
        {
        }

    }
}