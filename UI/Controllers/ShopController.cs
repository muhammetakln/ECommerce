using Core.Abstracts.IServices;
using Core.Concretes.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UI.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly IShopService service;
        private readonly IOrderService orderService;
        private readonly UserManager<Customer> userManager;

        public ShopController(IShopService service, IOrderService orderService, UserManager<Customer> userManager)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // ============================================
        // SEPET SAYFASI
        // ============================================

        // GET: /Shop/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR] User not authenticated");
                    return RedirectToAction("Login", "Accounts");
                }

                string? uid = userManager.GetUserId(User);

                if (string.IsNullOrEmpty(uid))
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR] User ID is null");
                    return RedirectToAction("Login", "Accounts");
                }

                var currentCart = await service.GetCartAsync(uid);

                if (currentCart == null)
                {
                    currentCart = new Core.Concretes.Dtos.CartDto
                    {
                        CustomerId = uid,
                        Items = new List<Core.Concretes.Dtos.CartItemDto>()
                    };
                }

                return View(currentCart);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CRITICAL ERROR] Index: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[STACK TRACE] {ex.StackTrace}");
                TempData["Error"] = "Sepet yüklenirken hata oluştu.";
                return RedirectToAction("Index", "Home");
            }
        }

        // ============================================
        // SEPETE EKLE / MİKTAR GÜNCELLE
        // ============================================

        // GET: /Shop/AddToCart?pid=5&quantity=1&returnUrl=/
        public async Task<IActionResult> AddToCart(int pid, int quantity = 1, string returnUrl = "/")
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    TempData["Error"] = "Lütfen önce giriş yapınız.";
                    return RedirectToAction("Login", "Accounts");
                }

                string? uid = userManager.GetUserId(User);

                if (string.IsNullOrEmpty(uid))
                {
                    TempData["Error"] = "Kullanıcı tanımlanamadı.";
                    return RedirectToAction("Login", "Accounts");
                }

                if (quantity == 0)
                {
                    TempData["Error"] = "Miktar 0 olamaz.";
                    return Redirect(returnUrl);
                }

                await service.AddToCartAsync(uid, pid, quantity);

                if (quantity > 0)
                {
                    TempData["Success"] = "Ürün sepete eklendi!";
                }
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Sepete eklenirken hata oluştu.";
                System.Diagnostics.Debug.WriteLine($"[ERROR] AddToCart: {ex.Message}");
            }

            // Sepet sayfasından geldiyse veya azaltma ise sepette kal
            if (returnUrl.Contains("/shop", StringComparison.OrdinalIgnoreCase) || quantity < 0)
            {
                return RedirectToAction("Index");
            }

            return Redirect(returnUrl);
        }

        // ============================================
        // SEPETTEN ÇIKART
        // ============================================

        // GET/POST: /Shop/RemoveFromCart?pid=5
        public async Task<IActionResult> RemoveFromCart(int pid)
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    TempData["Error"] = "Lütfen giriş yapınız.";
                    return RedirectToAction("Login", "Accounts");
                }

                string? uid = userManager.GetUserId(User);

                if (string.IsNullOrEmpty(uid))
                {
                    TempData["Error"] = "Kullanıcı tanımlanamadı.";
                    return RedirectToAction("Login", "Accounts");
                }

                await service.RemoveFromCartAsync(uid, pid);
                TempData["Success"] = "Ürün sepetten kaldırıldı.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ürün kaldırılırken hata oluştu.";
                System.Diagnostics.Debug.WriteLine($"[ERROR] RemoveFromCart: {ex.Message}");
            }

            return RedirectToAction("Index");
        }

        // ============================================
        // ÖDEMEYE GEÇ (SİPARİŞ OLUŞTUR)
        // ============================================

        // POST: /Shop/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    TempData["Error"] = "Lütfen giriş yapınız.";
                    return RedirectToAction("Login", "Accounts");
                }

                string? uid = userManager.GetUserId(User);

                if (string.IsNullOrEmpty(uid))
                {
                    TempData["Error"] = "Kullanıcı tanımlanamadı.";
                    return RedirectToAction("Login", "Accounts");
                }

                // ✅ Sipariş oluştur - OrderService.CreateOrderAsync çağrılır
                // Service içinde:
                // 1. Sepet bulunur
                // 2. Order oluşturulur (Status = Pending)
                // 3. OrderItem'lar oluşturulur
                // 4. Sepet pasif yapılır (cart.Active = false)
                // 5. Order.Id döndürülür
                int orderId = await orderService.CreateOrderAsync(uid);

                TempData["Success"] = $"Siparişiniz başarıyla oluşturuldu! Sipariş No: {orderId}";

                // ✅ Sipariş detayına yönlendir
                return RedirectToAction("Index", "Order", new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                // Sepet boş hatası
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Sipariş oluşturulurken hata oluştu.";
                System.Diagnostics.Debug.WriteLine($"[ERROR] Checkout: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[STACK TRACE] {ex.StackTrace}");
                return RedirectToAction("Index");
            }
        }

        // ============================================
        // SEPETİ TEMİZLE
        // ============================================

        // POST: /Shop/ClearCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                if (User?.Identity?.IsAuthenticated != true)
                {
                    TempData["Error"] = "Lütfen giriş yapınız.";
                    return RedirectToAction("Login", "Accounts");
                }

                string? uid = userManager.GetUserId(User);

                if (string.IsNullOrEmpty(uid))
                {
                    TempData["Error"] = "Kullanıcı tanımlanamadı.";
                    return RedirectToAction("Login", "Accounts");
                }

                var cart = await service.GetCartAsync(uid);

                if (cart != null && cart.Items.Any())
                {
                    foreach (var item in cart.Items.ToList())
                    {
                        await service.RemoveFromCartAsync(uid, item.ProductId);
                    }
                }

                TempData["Success"] = "Sepet temizlendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Sepet temizlenirken hata oluştu.";
                System.Diagnostics.Debug.WriteLine($"[ERROR] ClearCart: {ex.Message}");
            }

            return RedirectToAction("Index");
        }
    }
}