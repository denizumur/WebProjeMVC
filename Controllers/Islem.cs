using berber.Models;
using Microsoft.AspNetCore.Mvc;
namespace berber.Controllers
{
    public class Islem : Controller
    {
        Context c=new Context();
        public IActionResult Index()
        {
            var degerler=c.Islemler.ToList();
            return View(degerler);
        }
    }
}
