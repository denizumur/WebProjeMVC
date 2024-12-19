using Microsoft.AspNetCore.Mvc;

namespace berber.Controllers
{
    public class RandevuController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
