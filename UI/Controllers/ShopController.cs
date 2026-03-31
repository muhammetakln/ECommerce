using Core.Abstracts.IServices;
using Core.Concretes.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace UI.Controllers
{
    [Authorize]
    public class ShopController : Controller
    {
        private readonly IShopService service;
        private readonly UserManager<Customer> userManager;

        public ShopController(IShopService service, UserManager<Customer> userManager)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // ✅ FIX 1: User null mu kontrol et
                if (User?.Identity?.IsAuthenticated != true)
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR] User not authenticated");
                    return RedirectToAction("Login", "Accounts");
                }

                // ✅ FIX 2: User ID'sini al
                string? uid = userManager.GetUserId(User);

                // ✅ FIX 3: User ID null mu kontrol et
                if (string.IsNullOrEmpty(uid))
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR] User ID is null");
                    return RedirectToAction("Login", "Accounts");
                }

                // ✅ FIX 4: Service null mu kontrol et
                if (service == null)
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR] Service is null");
                    return StatusCode(500, "Service not available");
                }

                var currentCart = await service.GetCartAsync(uid);

                // ✅ FIX 5: Cart null mu kontrol et
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
                return RedirectToAction("index", "home");
            }
        }

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
                    TempData["Success"] = "Ürün sepete eklendi!";
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
                return RedirectToAction("index");

            return Redirect(returnUrl);
        }

        public async Task<IActionResult> RemoveFromCart(int pid)
        {
            try
            {
                // ✅ FIX 1: User null mu kontrol et
                if (User?.Identity?.IsAuthenticated != true)
                {
                    TempData["Error"] = "Lütfen giriş yapınız.";
                    return RedirectToAction("Login", "Accounts");
                }

                // ✅ FIX 2: User ID'sini al
                string? uid = userManager.GetUserId(User);

                // ✅ FIX 3: User ID null mu kontrol et
                if (string.IsNullOrEmpty(uid))
                {
                    TempData["Error"] = "Kullanıcı tanımlanamadı.";
                    return RedirectToAction("Login", "Accounts");
                }

                // ✅ FIX 4: Service null mu kontrol et
                if (service == null)
                {
                    TempData["Error"] = "Sistem hatası.";
                    return RedirectToAction("index");
                }

                await service.RemoveFromCartAsync(uid, pid);
                TempData["Success"] = "Ürün sepetten kaldırıldı.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ürün kaldırılırken hata oluştu.";
                System.Diagnostics.Debug.WriteLine($"[ERROR] RemoveFromCart: {ex.Message}");
            }

            return RedirectToAction("index");
        }
    }
}