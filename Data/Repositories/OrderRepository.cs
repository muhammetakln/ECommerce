using Core.Abstracts.IRepositories;
using Core.Concretes.Entities;
using Utils.Generics;

namespace Data.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ShopContext db) : base(db)
        {

        }
    }
}