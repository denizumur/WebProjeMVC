using Microsoft.AspNetCore.Mvc;

namespace berber.Controllers
{
    public class Hakkimizda : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
