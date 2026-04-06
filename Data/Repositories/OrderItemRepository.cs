using Core.Abstracts.IRepositories;
using Core.Concretes.Entities;
using Utils.Generics;

namespace Data.Repositories
{
    public class OrderItemRepository : Repository<OrderITem>, IOrderItemRepository
    {
        public OrderItemRepository(ShopContext db) : base(db)
        {

        }
    }
}