using Core.Abstracts.IRepositories;
using Core.Concretes.Entities;
using Microsoft.EntityFrameworkCore;
using Utils.Generics;

namespace Data.Repositories
{
    public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
    {
        public ProductReviewRepository(ShopContext db) : base(db)
        {
        }


    }
}