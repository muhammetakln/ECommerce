using Core.Abstracts.IServices;
using Core.Concretes.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks;

namespace UI.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAuthService service;

        public AccountsController(IAuthService service)
        {
            this.service = service;
        }

        // Profil ekranı :
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login(string returnUrl = "/")
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await service.LoginAsync(model);
            if (result.IsSuccess)
            {
                return Redirect(returnUrl);

            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View(model);

        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]//güvenlik için token doğrulama.

        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await service.RegisterAsync(model);
            if (result.IsSuccess)
            {
                return RedirectToAction("Login");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await service.LogoutAsync();
            //çıkış işlemi başarılı ise home indexe  git 
            return RedirectToAction("index", "home");
        }
    }
}
