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
                    System.Diagnostics.Debug.WriteLine(
                        $"[UPDATE] CartItem updated - CartId: {cart.Id}, ProductId: {productId}, NewQty: {cartItem.Quantity}"
                    );
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
                    System.Diagnostics.Debug.WriteLine(
                        $"[CREATE] CartItem created - CartId: {cart.Id}, ProductId: {productId}, Qty: {quantity}"
                    );
                }

                var commitResult = await unitOfWork.CommitAsync();
                if (!commitResult.IsSuccess)
                    throw new Exception($"Sepete eklenirken hata: {commitResult.Message}");

                System.Diagnostics.Debug.WriteLine(
                    $"[SUCCESS] AddToCartAsync completed - CustomerId: {customerId}, ProductId: {productId}"
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[ERROR] AddToCartAsync: {ex.Message}\nStackTrace: {ex.StackTrace}"
                );
                throw;
            }
        }

        public async Task<CartDto> GetCartAsync(string customerId)
        {
            try
            {
                // ✅ FIX: Items ve Products'ı eager load et
                // Aksi halde AutoMapper mapping başarısız olur
                var cartResult = await unitOfWork.CartRepository.FindFirstAsync(
                    c => c.CustomerId == customerId,
                    "Items",           // ← Include CartItems collection
                    "Items.Product"    // ← Include Product her CartItem'da
                );

                if (!cartResult.IsSuccess || cartResult.Data == null)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[WARNING] Cart not found for customerId: {customerId}, creating new one"
                    );
                    // Sepet yok, yeni bir boş sepet dön
                    return new CartDto
                    {
                        CustomerId = customerId,
                        Items = new List<CartItemDto>()
                    };
                }

                var cart = cartResult.Data;
                System.Diagnostics.Debug.WriteLine(
                    $"[INFO] Cart loaded - CustomerId: {customerId}, ItemCount: {cart.Items?.Count ?? 0}"
                );

                var cartDto = mapper.Map<CartDto>(cart);
                return cartDto;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[ERROR] GetCartAsync: {ex.Message}\nStackTrace: {ex.StackTrace}"
                );
                throw;
            }
        }

        public async Task RemoveFromCartAsync(string customerId, int productId)
        {
            try
            {
                var cart = await getCart(customerId);

                if (cart == null)
                    throw new InvalidOperationException("Sepet bulunamadı");

                var cartItemResult = await unitOfWork.CartItemRepository.FindFirstAsync(
                    x => x.CartId == cart.Id && x.ProductId == productId
                );

                if (cartItemResult.IsSuccess && cartItemResult.Data != null)
                {
                    await unitOfWork.CartItemRepository.DeleteAsync(cartItemResult.Data);
                    var commitResult = await unitOfWork.CommitAsync();

                    if (!commitResult.IsSuccess)
                        throw new Exception($"Silme işlemi başarısız: {commitResult.Message}");

                    System.Diagnostics.Debug.WriteLine(
                        $"[DELETE] CartItem removed - CartId: {cart.Id}, ProductId: {productId}"
                    );
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[WARNING] CartItem not found to remove - CartId: {cart.Id}, ProductId: {productId}"
                    );
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[ERROR] RemoveFromCartAsync: {ex.Message}\nStackTrace: {ex.StackTrace}"
                );
                throw;
            }
        }


        private async Task<Cart> getCart(string customerId)
        {
            var result = await unitOfWork.CartRepository.FindManyAsync(
                c => c.CustomerId == customerId && c.Active,
                "Items.Product"  // ← Eager loading
            );

            if (result.IsSuccess)
            {
                return result.Data.FirstOrDefault();
            }
            else
            {
                var newCart = new Cart { CustomerId = customerId };
                await unitOfWork.CartRepository.CreateAsync(newCart);
                var commitResult = await unitOfWork.CommitAsync();
                if (commitResult.IsSuccess)
                {
                    return newCart;
                }
                else
                {
                    throw new Exception("Sepet oluşturulurken bir hata oluştu.");
                }
            }
        }
    }
}