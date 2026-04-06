using AutoMapper;
using Core.Abstracts;
using Core.Abstracts.IServices;
using Core.Concretes.Dtos;
using Core.Concretes.Entities;
using Core.Concretes.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        // ============================================
        // SİPARİŞ DURUMU GÜNCELLE
        // ============================================

        public async Task<int> ChangeOrderStatusAsync(int orderId, OrderStatus status)
        {
            var result = await unitOfWork.OrderRepository.FindManyAsync(o => o.Id == orderId);

            if (result.IsSuccess)
            {
                var order = result.Data.FirstOrDefault();

                if (order != null)
                {
                    order.Status = status;
                    await unitOfWork.OrderRepository.UpdateAsync(order);
                    await unitOfWork.CommitAsync();

                    return order.Id;
                }
            }

            throw new KeyNotFoundException($"Sipariş bulunamadı: {orderId}");
        }

        // ============================================
        // SİPARİŞ OLUŞTUR - DÜZELTİLDİ
        // ============================================

        public async Task<int> CreateOrderAsync(string customerId)
        {
            // ✅ Active kontrolünü KALDIRDIK - tüm sepetleri ara
            var cartResult = await unitOfWork.CartRepository.FindManyAsync(
                c => c.CustomerId == customerId,  // && c.Active kaldırıldı
                "Items.Product"
            );

            System.Diagnostics.Debug.WriteLine($"[DEBUG] CustomerId: {customerId}");
            System.Diagnostics.Debug.WriteLine($"[DEBUG] cartResult.IsSuccess: {cartResult.IsSuccess}");

            if (!cartResult.IsSuccess)
            {
                throw new InvalidOperationException("Sepet bulunamadı (IsSuccess = false).");
            }

            var cart = cartResult.Data?.FirstOrDefault();

            if (cart != null)
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] cart.Id: {cart.Id}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] cart.Active: {cart.Active}");
                System.Diagnostics.Debug.WriteLine($"[DEBUG] cart.Items Count: {cart.Items?.Count ?? 0}");
            }

            if (cart == null)
            {
                throw new InvalidOperationException("Sepet bulunamadı.");
            }

            // ✅ Sepet pasifse aktif yap
            if (!cart.Active)
            {
                System.Diagnostics.Debug.WriteLine($"[INFO] Sepet pasifti, aktif yapılıyor...");
                cart.Active = true;
                await unitOfWork.CartRepository.UpdateAsync(cart);
                await unitOfWork.SaveChangesAsync();
            }

            if (cart.Items == null || !cart.Items.Any())
            {
                throw new InvalidOperationException("Sepetinizde ürün bulunmuyor.");
            }

            // ✅ Order oluştur
            var order = new Order
            {
                CustomerId = customerId,
                CartId = cart.Id,
                Status = OrderStatus.Pending
            };

            await unitOfWork.OrderRepository.CreateAsync(order);
            await unitOfWork.SaveChangesAsync();

            if (order.Id == 0)
            {
                throw new InvalidOperationException("Order.Id oluşturulamadı!");
            }

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Order.Id: {order.Id}");

            // ✅ OrderItem'lar oluştur
            var orderItems = from item in cart.Items
                             select new OrderITem
                             {
                                 OrderId = order.Id,
                                 ProductId = item.ProductId,
                                 Quantity = item.Quantity,
                                 ListPrice = item.Product.Price,
                                 DiscountValue = item.Product.Price * item.Product.DiscountRate / 100
                             };

            await unitOfWork.OrderItemRepository.CreateManyAsync(orderItems);

            // ✅ Sepeti tekrar pasif yap
            cart.Active = false;
            await unitOfWork.CartRepository.UpdateAsync(cart);
            await unitOfWork.SaveChangesAsync();

            System.Diagnostics.Debug.WriteLine($"[SUCCESS] Order created: {order.Id}");

            return order.Id;
        }

        // ============================================
        // SİPARİŞ GÖRÜNTÜLE
        // ============================================

        public async Task<OrderDto> GetOrdersAsync(int orderId, string customerId)
        {
            // OrderId varsa o siparişi getir
            if (orderId > 0)
            {
                var orderResult = await unitOfWork.OrderRepository.FindManyAsync(
                    o => o.Id == orderId,
                    "Items.Product.Images"  // ✅ Images de dahil
                );

                if (orderResult.IsSuccess && orderResult.Data.Any())
                {
                    var order = orderResult.Data.FirstOrDefault();

                    // CustomerId kontrolü
                    if (!string.IsNullOrEmpty(customerId) && order.CustomerId != customerId)
                    {
                        throw new UnauthorizedAccessException("Bu siparişi görüntüleme yetkiniz yok.");
                    }

                    return mapper.Map<OrderDto>(order);
                }

                throw new KeyNotFoundException($"Sipariş bulunamadı: {orderId}");
            }

            // CustomerId varsa o müşterinin siparişlerini getir
            if (!string.IsNullOrEmpty(customerId))
            {
                var ordersResult = await unitOfWork.OrderRepository.FindManyAsync(
                    o => o.CustomerId == customerId,
                    "Items.Product.Images"  // ✅ Images de dahil
                );

                if (ordersResult.IsSuccess && ordersResult.Data.Any())
                {
                    var firstOrder = ordersResult.Data.FirstOrDefault();
                    return mapper.Map<OrderDto>(firstOrder);
                }

                throw new KeyNotFoundException($"Müşteri için sipariş bulunamadı: {customerId}");
            }

            throw new ArgumentException("OrderId veya CustomerId belirtilmelidir.");
        }
    }
}