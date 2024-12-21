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
			if (ModelState.IsValid)
			{
				// Kullanıcı bilgilerini veritabanına ekle
				user.UyelikTarihi = DateTime.Now;
				c.Kullanicilar.Add(user);
				c.SaveChanges();

				return RedirectToAction("KullaniciGoruntuleme","Admin"); // Başarılı bir yönlendirme
			}

			var errors = ModelState.Values.SelectMany(v => v.Errors);
			foreach (var error in errors)
			{
				Console.WriteLine(error.ErrorMessage); // Hataları kontrol edin
			}

			// Hatalı ise aynı formu tekrar göster
			return View("Kayit", user); // Form sayfasını tekrar yükler
		}
	}
}
