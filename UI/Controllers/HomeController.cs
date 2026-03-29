using Core.Abstracts.IServices;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
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

        public async Task<IActionResult> Index()
        {
            var result=await service.GetProductAsync();
            if (result.IsSuccess)
            {
                return View(result.Data);
            }
            return View();
        }
        public async Task<IActionResult> Details(int id)
        {
            System.Diagnostics.Debug.WriteLine($"Details çağrıldı, ID: {id}");

            var result = await service.GetProductDetailAsync(id);

            System.Diagnostics.Debug.WriteLine($"IsSuccess: {result.IsSuccess}");
            System.Diagnostics.Debug.WriteLine($"Message: {result.Message}");
            System.Diagnostics.Debug.WriteLine($"Data null mu: {result.Data == null}");

            if (result.IsSuccess)
            {
                System.Diagnostics.Debug.WriteLine($"Ürün adı: {result.Data.Name}");
                return View(result.Data);
            }

            return NotFound(result.Message);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
