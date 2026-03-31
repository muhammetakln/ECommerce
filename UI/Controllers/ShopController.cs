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
            this.service = service;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            string? uid = userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(uid))
            {
                var currentCart = await service.GetCartAsync(uid);
                return View(currentCart);
            }

            return RedirectToAction("index", "home");
        }

        public async Task<IActionResult> AddToCart(int pid, int quantity = 1)
        {
            string? uid = userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(uid))
            {
                await service.AddToCartAsync(uid, pid, quantity);
            }

            return RedirectToAction("index");
        }

        public async Task<IActionResult> RemoveFromCart(int pid)
        {
            string? uid = userManager.GetUserId(User);

            if (!string.IsNullOrEmpty(uid))
            {
                await service.RemoveFromCartAsync(uid, pid);
            }
            return RedirectToAction("index");
        }
    }
}
