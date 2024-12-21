using berber.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace berber.Controllers
{
	public class AdminController : Controller
	{
		Context c = new Context();
		public IActionResult Index()
		{

			return View();
		}

		public IActionResult CalisanIslemleri()
		{
			return View();
		}
		public IActionResult HizmetIslemleri()
		{
			return View();
		}
		public IActionResult RandevuIslemleri()
		{
			return View();
		}
		public IActionResult KullaniciIslemleri()
		{
			return View();
		}
		public IActionResult CalisanGoruntuleme()
		{
			var calisanlar = c.Calisanlar.ToList();
			return View(calisanlar);
		}
		public IActionResult HizmetGoruntuleme()
		{
			var degerler = c.Islemler.ToList();
			return View(degerler);
		}


		//randevu listeleme yapılacak
		public IActionResult RandevuGoruntuleme()
		{
			var randevular = c.Randevular
				.Include(x => x.Islem)          // Islem ilişkisini dahil et
				                              // Kullanici ilişkisini dahil et
				.Include(x => x.Calisan)        // Calisan ilişkisini dahil et
				.ToList();                      // Veritabanındaki tüm randevuları alıyoruz

			return View(randevular);            // View'a gönderiyoruz
		}

		//kullanici listeleme yapılacak 
		public IActionResult KullaniciGoruntuleme()
		{
			var kullanicilar = c.Kullanicilar.ToList();
			return View(kullanicilar);
		}

		public IActionResult KullaniciEkle(Kullanici user)
		{
			if (ModelState.IsValid)
			{
				// Kullanıcı bilgilerini veritabanına ekle
				user.UyelikTarihi = DateTime.Now;
				c.Kullanicilar.Add(user);
				c.SaveChanges();

				return View("KullaniciGoruntuleme"); // Başarılı bir yönlendirme
			}

			return View(user); // Hatalıysa formu tekrar göster
		}
	}
}
