using Core.Abstracts.IServices;
using Core.Concretes.Entities;
using Core.Concretes.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace UI.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService service;
        private readonly UserManager<Customer> userManager;

        public OrderController(IOrderService service, UserManager<Customer> userManager)
        {
            this.service = service;
            this.userManager = userManager;
        }

        // ============================================
        // SİPARİŞLERİM VEYA SİPARİŞ DETAYI
        // ============================================

        // GET: /Order veya /Order/Index veya /Order/Index/5
        [HttpGet]
        [Route("Order")]
        [Route("Order/Index")]
        [Route("Order/Index/{id?}")]
        public async Task<IActionResult> Index(int? id)
        {
            try
            {
                string? uid = userManager.GetUserId(User);

                if (string.IsNullOrEmpty(uid))
                {
                    return RedirectToAction("Login", "Account");
                }

                // id varsa tek sipariş detayı
                if (id.HasValue)
                {
                    var result = await service.GetOrdersAsync(id.Value, uid);

                    if (result != null)
                    {
                        return View(result);  // ✅ Index view kullan
                    }

                    return NotFound();
                }

                // id yoksa kullanıcının tüm siparişleri
                var orders = await service.GetOrdersAsync(0, uid);
                return View(orders);
            }
            catch (KeyNotFoundException)
            {
                TempData["Info"] = "Henüz hiç siparişiniz yok.";
                return View();
            }
            catch (UnauthorizedAccessException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Hata oluştu.";
                System.Diagnostics.Debug.WriteLine($"[ERROR] Index: {ex.Message}");
                return RedirectToAction("Index", "Home");
            }
        }

        // ============================================
        // SİPARİŞ OLUŞTUR
        // ============================================

        // POST: /Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Order/Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                string? uid = userManager.GetUserId(User);

                if (string.IsNullOrEmpty(uid))
                {
                    return RedirectToAction("Login", "Account");
                }

                // Sipariş oluştur
                int orderId = await service.CreateOrderAsync(uid);

                TempData["Success"] = $"Siparişiniz başarıyla oluşturuldu! Sipariş No: {orderId}";

                // Sipariş detayına yönlendir
                return RedirectToAction("Index", new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Shop");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Sipariş oluşturulurken hata oluştu.";
                System.Diagnostics.Debug.WriteLine($"[ERROR] Create: {ex.Message}");
                return RedirectToAction("Index", "Shop");
            }
        }

        // ============================================
        // SİPARİŞ DURUMU DEĞİŞTİR (ADMIN)
        // ============================================

        // POST: /Order/ChangeStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [Route("Order/ChangeStatus")]
        public async Task<IActionResult> ChangeStatus(int id, int status)
        {
            try
            {
                await service.ChangeOrderStatusAsync(id, (OrderStatus)status);

                TempData["Success"] = "Sipariş durumu güncellendi.";
                return RedirectToAction("Index", new { id });
            }
            catch (KeyNotFoundException)
            {
                TempData["Error"] = "Sipariş bulunamadı.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Durum güncellenirken hata oluştu.";
                System.Diagnostics.Debug.WriteLine($"[ERROR] ChangeStatus: {ex.Message}");
                return RedirectToAction("Index", new { id });
            }
        }
    }
}