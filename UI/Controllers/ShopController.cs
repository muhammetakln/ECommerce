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

        public async Task<IActionResult> AddToCart(int pid, int quantity = 1)
        {
            try
            {
                // ✅ FIX 1: User null mu kontrol et
                if (User?.Identity?.IsAuthenticated != true)
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR] User not authenticated in AddToCart");
                    TempData["Error"] = "Lütfen önce giriş yapınız.";
                    return RedirectToAction("Login", "Accounts");
                }

                // ✅ FIX 2: User ID'sini al
                string? uid = userManager.GetUserId(User);

                // ✅ FIX 3: User ID null mu kontrol et
                if (string.IsNullOrEmpty(uid))
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR] User ID is null in AddToCart");
                    TempData["Error"] = "Kullanıcı tanımlanamadı.";
                    return RedirectToAction("Login", "Accounts");
                }

                // ✅ FIX 4: Service null mu kontrol et
                if (service == null)
                {
                    System.Diagnostics.Debug.WriteLine("[ERROR] Service is null in AddToCart");
                    TempData["Error"] = "Sistem hatası.";
                    return RedirectToAction("index");
                }

                // ✅ FIX 5: Quantity validation
                if (quantity <= 0)
                {
                    TempData["Error"] = "Miktar 0'dan büyük olmalıdır.";
                    return RedirectToAction("index");
                }

                System.Diagnostics.Debug.WriteLine(
                    $"[INFO] AddToCart: UID={uid}, ProductID={pid}, Qty={quantity}"
                );

                await service.AddToCartAsync(uid, pid, quantity);

                TempData["Success"] = $"Ürün sepete eklendi!";
                System.Diagnostics.Debug.WriteLine(
                    $"[SUCCESS] AddToCart completed for UID={uid}, ProductID={pid}"
                );
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = $"Validation: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[VALIDATION ERROR] AddToCart: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = $"İşlem hatası: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[OPERATION ERROR] AddToCart: {ex.Message}");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Sepete eklenirken hata oluştu.";
                System.Diagnostics.Debug.WriteLine($"[CRITICAL ERROR] AddToCart: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[STACK TRACE] {ex.StackTrace}");
            }

            return RedirectToAction("index");
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