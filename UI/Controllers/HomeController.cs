using Core.Abstracts.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UI.Models;

namespace UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IShowroomService service;

        public HomeController(IShowroomService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Ana sayfa - Tüm ürünlerin listesi
        /// GET: /Home/Index
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var result = await service.GetProductAsync();
            if (result.IsSuccess)
            {
                return View(result.Data);
            }
            return View();
        }

        /// <summary>
        /// Ürün detay sayfası
        /// GET: /Home/Details/1
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Geçersiz ürün ID'si");
            }

            var result = await service.GetProductDetailAsync(id);

            if (!result.IsSuccess)
            {
                if (result.StatusCode == 404)
                    return NotFound(result.Message);

                return BadRequest(result.Message);
            }

            return View(result.Data);
        }

        /// <summary>
        /// Ürün düzenleme sayfası (Admin)
        /// GET: /Home/Edit/1
        /// </summary>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Geçersiz ürün ID'si");
            }

            var result = await service.GetProductDetailAsync(id);

            if (!result.IsSuccess)
            {
                if (result.StatusCode == 404)
                    return NotFound("Ürün bulunamadı");

                return BadRequest(result.Message);
            }

            return View(result.Data);
        }

        /// <summary>
        /// Ürün güncelleme işlemi (Admin)
        /// POST: /Home/Edit/1
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Core.Concretes.DTOs.ProductDetailDto dto)
        {
            if (id <= 0)
            {
                return BadRequest("Geçersiz ürün ID'si");
            }

            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await service.UpdateProductAsync(id, dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Message);
                return View(dto);
            }

            // Başarılı - Detay sayfasına yönlendir
            return RedirectToAction("Details", new { id = id });
        }

        /// <summary>
        /// Ürün silme işlemi (Admin)
        /// POST: /Home/Delete/1
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Geçersiz ürün ID'si");
            }

            var result = await service.DeleteProductAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            // Başarılı - Ana sayfaya yönlendir
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Gizlilik politikası sayfası
        /// GET: /Home/Privacy
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Hata sayfası
        /// GET: /Home/Error
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}