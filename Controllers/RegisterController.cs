using berber.Models;
using Microsoft.AspNetCore.Mvc;

namespace berber.Controllers
{
	public class RegisterController : Controller
	{
		Context c = new Context();
		public IActionResult Kayit()
		{


			return View();
		}
		[HttpPost]
		public IActionResult KullaniciEkle(Kullanici user)
		{
			// Model doğrulama kontrolü
			if (ModelState.IsValid)
			{
				// Kullanıcı adı ile ilgili kontrol
				var mevcutKullanici = c.Kullanicilar.FirstOrDefault(k => k.AdSoyad == user.AdSoyad);
				if (mevcutKullanici != null)
				{
					// Aynı kullanıcı adı varsa hata mesajı ekle
					
					ModelState.Clear();
					return View("Kayit"); // Hatalı ise aynı formu tekrar göster
				}

				// Kullanıcı bilgilerini veritabanına ekle
				user.UyelikTarihi = DateTime.Now;
				c.Kullanicilar.Add(user);
				c.SaveChanges();

				return RedirectToAction("Index", "Login"); // Başarılı bir yönlendirme
			}

			
			
			ModelState.Clear();
			return View("Kayit"); // Form sayfasını tekrar yükler
		}
	}
}
