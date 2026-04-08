using Core.Concretes.Dtos;
using Core.Concretes.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Core.Abstracts.IServices
{
    public interface IOrderService
    {
        // ============================================
        // SIPARIŞ OLUŞTURMA
        // ============================================

        /// <summary>
        /// Sepetteki ürünlerden yeni sipariş oluşturur
        /// </summary>
        Task<int> ChangeOrderStatusAsync(int orderId, OrderStatus status);

        /// <summary>
        /// Manuel sipariş oluşturur
        /// </summary>
        Task<int> CreateOrderAsync(string customerId);

        // ============================================
        // SIPARIŞ GÖRÜNTÜLEME
        // ============================================

        /// <summary>
        /// Müşterinin tüm siparişlerini getirir
        /// </summary>

        Task<OrderDto> GetOrdersAsync(int orderId, string customerId);

        /// <summary>
        /// Sipariş detayını getirir (Items dahil)
        /// </summary>
        Task CheckOutAsync(int orderId,HttpClient client);
    }
}


