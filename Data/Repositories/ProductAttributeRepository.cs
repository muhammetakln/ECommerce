using Core.Abstracts.IRepositories;
using Core.Concretes.Entities;
using Microsoft.EntityFrameworkCore;
using Utils.Generics;

namespace Data.Repositories
{
    public class ProductAttributeRepository : Repository<ProductAttribute>,
        IProductAttributeRepository
    {
        public ProductAttributeRepository(ShopContext db) : base(db)
        {
        }
    }
}