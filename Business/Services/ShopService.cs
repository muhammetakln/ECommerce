using AutoMapper;
using Core.Abstracts;
using Core.Abstracts.IServices;
using Core.Concretes.Dtos;
using Core.Concretes.DTOs;
using Core.Concretes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ShopService : IShopService
    {
        




        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ShopService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task AddToCartAsync(string customerId, int productId, int quantity = 1)
        {
            
            try
            {
                if (quantity <= 0)
                    throw new ArgumentException("Miktar 0'dan büyük olmalıdır");

                var cart = await getCart(customerId);

                if (cart == null)
                    throw new InvalidOperationException("Sepet alınamadı");

                var cartItemResult = await unitOfWork.CartItemRepository.FindFirstAsync(
                    x => x.CartId == cart.Id && x.ProductId == productId
                );

                if (cartItemResult.IsSuccess && cartItemResult.Data != null)
                {
                    // ÜRÜN ZATEN VAR: Miktarı artır
                    CartItem cartItem = cartItemResult.Data;
                    cartItem.Quantity += quantity;
                    await unitOfWork.CartItemRepository.UpdateAsync(cartItem);
                }
                else
                {
                    // ÜRÜN YOK: Yeni CartItem oluştur
                    CartItem newCartItem = new()
                    {
                        CartId = cart.Id,
                        ProductId = productId,
                        Quantity = quantity
                    };
                    await unitOfWork.CartItemRepository.CreateAsync(newCartItem);
                }

                var commitResult = await unitOfWork.CommitAsync();
                if (!commitResult.IsSuccess)
                    throw new Exception($"Sepete eklenirken hata: {commitResult.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERROR] AddToCartAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<CartDto> GetCartAsync(string customerId)
        {
            var cart = await getCart(customerId);
            return mapper.Map<CartDto>(cart);
        }

        public async Task RemoveFromCartAsync(string customerId, int productId)
        {
            var cart = await getCart(customerId);

            var cartItemResult = await unitOfWork.CartItemRepository.FindFirstAsync(x => x.CartId == cart.Id && x.ProductId == productId);

            if (cartItemResult.IsSuccess)
            {
                await unitOfWork.CartItemRepository.DeleteAsync(cartItemResult.Data);
                await unitOfWork.CommitAsync();
            }
        }


        private async Task<Cart> getCart(string customerId)
        {
            // Mevcut sepeti ara
            var result = await unitOfWork.CartRepository.FindFirstAsync(
                c => c.CustomerId == customerId
            );

            if (result.IsSuccess && result.Data != null)  // ← NULL CHECK ekle!
            {
                return result.Data;
            }

            // Sepet bulunamadı, yeni oluştur
            var newCart = new Cart
            {
                CustomerId = customerId
            };

            await unitOfWork.CartRepository.CreateAsync(newCart);
            var commitResult = await unitOfWork.CommitAsync();

            if (!commitResult.IsSuccess)
            {
                throw new Exception($"Sepet oluşturulurken hata: {commitResult.Message}");
            }

            // ✅ Yeni oluşturulan sepeti veritabanından tekrar al!
            var createdCartResult = await unitOfWork.CartRepository.FindFirstAsync(
                c => c.CustomerId == customerId
            );

            if (!createdCartResult.IsSuccess || createdCartResult.Data == null)
            {
                throw new Exception("Yeni sepet oluşturduktan sonra yüklenemedi");
            }

            return createdCartResult.Data;
        }
    }
}


