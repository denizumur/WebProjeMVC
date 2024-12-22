using berber.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;

namespace berber.Controllers
{
	public class LoginController : Controller
	{
		Context c = new Context();
		public IActionResult Index()
		{
			

			return View();
		}

		public IActionResult Giris(Kullanici kullanici)
		{

			if ((kullanici.AdSoyad == "melih" || kullanici.AdSoyad =="deniz" ) && kullanici.Sifre == "123") {
				return View("~/Views/Admin/Index.cshtml");


			}
			var mevcutIslem = c.Kullanicilar.FirstOrDefault(x => x.AdSoyad == kullanici.AdSoyad && x.Sifre == kullanici.Sifre);

			string kullaniciid = kullanici.AdSoyad;

			if (mevcutIslem != null)
			{

				HttpContext.Session.SetString("SesUsr", kullaniciid);
				TempData["uyari"] = "Başarılı giriş";
				return RedirectToAction("Anasayfa");
			}
			TempData["uyari"] = "Başarısız.";
			return RedirectToAction("Index");
		}

		public IActionResult Anasayfa() {

			if (HttpContext.Session.GetString("SesUsr") is not null)
			{

				return View();
			}

			TempData["uyari"] = "Lütfen login olunuz";
			return RedirectToAction("Index");
		}
		
	}
}
