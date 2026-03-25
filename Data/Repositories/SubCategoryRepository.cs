using Core.Abstracts.IRepositories;
using Core.Concretes.Entities;
using Microsoft.EntityFrameworkCore;
using Utils.Generics;

namespace Data.Repositories
{
    public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
    {
        public SubCategoryRepository(ShopContext db) : base(db)
        {
        }
    }
}