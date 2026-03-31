using Core.Concretes.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Abstracts.IServices
{
    public interface IShopService
    {
        Task<CartDto> GetCartAsync(string customerId);
        Task AddToCartAsync(string customerId, int productId,int quantity=1);
        Task RemoveFromCartAsync(string customerId, int productId);

    }
}
