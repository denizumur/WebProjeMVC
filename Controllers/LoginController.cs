using berber.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace berber.Controllers
{
	public class LoginController : Controller
	{
		public IActionResult Index()
		{
			

			return View();
		}

		public IActionResult Giris(Kullanici kullanici)
		{

			if ((kullanici.AdSoyad == "melih" || kullanici.AdSoyad =="deniz" ) && kullanici.Sifre == "123") {
				return View("~/Views/Admin/Index.cshtml");
			}

			return RedirectToAction("Index");
		}
	}
}
