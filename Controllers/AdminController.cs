using berber.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

		 
		public IActionResult KullaniciGoruntuleme()
		{
			var kullanicilar = c.Kullanicilar.ToList();
			return View(kullanicilar);
		}

		[HttpGet]
		public IActionResult KullaniciEkle()
		{

			return View();
		}
		[HttpPost]
		public IActionResult KullaniciEkle(Kullanici user)
		{
			var mevcutIslem = c.Kullanicilar.FirstOrDefault(x => x.AdSoyad == user.AdSoyad && x.Sifre == user.Sifre);

			if (mevcutIslem != null)
			{
				
				TempData["HataMesaji"] = user.AdSoyad + " isimli kullanıcı daha önce tanımlanmış.";

				ModelState.Clear();

				return View("KullaniciEkle"); 
			}

			user.UyelikTarihi = DateTime.Now;
			c.Kullanicilar.Add(user);
			c.SaveChanges();
			return RedirectToAction("KullaniciGoruntuleme");
		}

		[HttpGet]
		public IActionResult HizmetEkle()
		{

			return View();
		}
		[HttpPost]
		public IActionResult HizmetEkle(Islem islem)
		{
			
			var mevcutIslem = c.Islemler.FirstOrDefault(x => x.IslemAdi == islem.IslemAdi && x.Ucret == islem.Ucret);

			if (mevcutIslem != null)
			{
				
				TempData["HataMesaji"] = "Bu hizmet daha önce tanımlanmış.";

				ModelState.Clear();

				return View("HizmetEkle"); // Formu boş bir modelle yolla 
			}

			
			c.Islemler.Add(islem);
			c.SaveChanges();
			return RedirectToAction("HizmetGoruntuleme");
		}

		[HttpGet]
		public IActionResult CalisanEkle()
		{

			return View();
		}
		[HttpPost]
		public IActionResult CalisanEkle(Calisan calisan)
		{

			var mevcutIslem = c.Calisanlar.FirstOrDefault(x => x.AdSoyad == calisan.UzmanlikAlanlari && x.UygunlukSaatleri == calisan.UygunlukSaatleri);

			if (mevcutIslem != null)
			{

				TempData["HataMesaji"] = "Bu çalışan daha önce tanımlanmış.";

				ModelState.Clear();

				return View("CalisanEkle"); // Formu boş bir modelle yolla 
			}


			c.Calisanlar.Add(calisan);
			c.SaveChanges();
			return RedirectToAction("CalisanGoruntuleme");
		}

		[HttpGet]
		public IActionResult RandevuEkle()
		{
			ViewBag.Calisanlar = c.Calisanlar.ToList();
			ViewBag.Islemler = c.Islemler.ToList();
			return View();
		}
		[HttpPost]
		public IActionResult RandevuEkle(Randevu model)
		{
			// Formdan gelen model doğrulanıyor
			if (!ModelState.IsValid)
			{
				// Hatalıysa formu yeniden yükler
				TempData["HataMesaji"] = "Lütfen tüm alanları doğru doldurun.";
				ViewBag.Calisanlar = c.Calisanlar.ToList();
				ViewBag.Islemler = c.Islemler.ToList();
				return View("RandevuEkle", model);
			}

			

			// Yeni randevuyu oluştur
			
			// Veritabanına ekle
			c.Randevular.Add(model);
			c.SaveChanges();

			TempData["BasariMesaji"] = "Randevu başarıyla kaydedildi.";
			return RedirectToAction("RandevuGoruntuleme");
		}

		public IActionResult CalisanSilme()
		{
			var calisanlar = c.Calisanlar.ToList();
			return View(calisanlar);
		}
		public IActionResult CalisanSil(int id)
		{
			var calisan = c.Calisanlar.Find(id);
			if (calisan != null)
			{
				c.Calisanlar.Remove(calisan);
				c.SaveChanges();
				TempData["BasariMesaji"] = "Çalışan silme başarılı.";
				return RedirectToAction("CalisanSilme");
			}
			TempData["BasariMesaji"] = "Çalışan silme başarısız.";
			return RedirectToAction("CalisanSilme");
		}
		public IActionResult CalisanGuncelleme()
		{
			var calisanlar = c.Calisanlar.ToList();
			return View(calisanlar);
		}

		[HttpGet]
		public IActionResult CalisanGuncelle(int id)
		{
			var calisan = c.Calisanlar.Find(id);
			if (calisan == null)
			{
				return RedirectToAction("CalisanGuncelleme");
			}
			return View(calisan);
		}

		[HttpPost]
		public IActionResult CalisanGuncelle(Calisan cal)
		{
			var calisan = c.Calisanlar.Find(cal.CalisanID);
			if (calisan != null)
			{
				calisan.AdSoyad = cal.AdSoyad;
				calisan.UygunlukSaatleri = cal.UygunlukSaatleri;
				calisan.UzmanlikAlanlari = cal.UzmanlikAlanlari;

				c.Entry(calisan).State = EntityState.Modified;
				c.Update(calisan);
				c.SaveChanges();
			}
			return RedirectToAction("CalisanGuncelleme");
		}


		public IActionResult HizmetSilme()
		{
			var hizmetler = c.Islemler.ToList();
			return View(hizmetler);
		}
		public IActionResult HizmetSil(int id)
		{
			var hizmet = c.Islemler.Find(id);
			if (hizmet != null)
			{
				c.Islemler.Remove(hizmet);
				c.SaveChanges();
				TempData["BasariMesaji"] = "Hizmet silme başarılı.";
				return RedirectToAction("HizmetSilme");
			}
			TempData["BasariMesaji"] = "Hizmet silme başarısız.";
			return RedirectToAction("HizmetSilme");
		}


		public IActionResult HizmetGuncelleme()
		{
			var hizmetler = c.Islemler.ToList();
			return View(hizmetler);
		}

		[HttpGet]
		public IActionResult HizmetGuncelle(int id)
		{
			var hizmet = c.Islemler.Find(id);
			if (hizmet == null)
			{
				return RedirectToAction("HizmetGuncelleme");
			}
			return View(hizmet);
		}

		[HttpPost]
		public IActionResult HizmetGuncelle(Islem islem)
		{
			var hizmet = c.Islemler.Find(islem.IslemID);
			if (hizmet != null)
			{
				hizmet.IslemAdi = islem.IslemAdi;
				hizmet.Sure = islem.Sure;
				hizmet.Ucret = islem.Ucret;

				c.Entry(hizmet).State = EntityState.Modified;
				c.Update(hizmet);
				c.SaveChanges();
				TempData["BasariMesaji"] = "saveledim genc.";
			}
			
			return RedirectToAction("HizmetGuncelleme");
		}



		public IActionResult KullaniciSilme()
		{
			var kullanicilar = c.Kullanicilar.ToList();
			return View(kullanicilar);
		}
		public IActionResult KullaniciSil(int id)
		{
			var kullanici = c.Kullanicilar.Find(id);
			if (kullanici != null)
			{
				c.Kullanicilar.Remove(kullanici);
				c.SaveChanges();
				TempData["BasariMesaji"] = "Kullanıcı silme başarılı.";
				return RedirectToAction("KullaniciSilme");
			}
			TempData["BasariMesaji"] = "Kullanıcı silme başarısız.";
			return RedirectToAction("KullaniciSilme");
		}

		public IActionResult KullaniciGuncelleme()
		{
			var kullanicilar = c.Kullanicilar.ToList();
			return View(kullanicilar);
		}

		[HttpGet]
		public IActionResult KullaniciGuncelle(int id)
		{
			var kullanici = c.Kullanicilar.Find(id);
			if (kullanici == null)
			{
				return RedirectToAction("KullaniciGuncelleme");
			}
			return View(kullanici);
		}

		[HttpPost]
		public IActionResult KullaniciGuncelle(Kullanici user)
		{
			var kullanici = c.Kullanicilar.Find(user.KullaniciID);
			if (kullanici != null)
			{
				kullanici.AdSoyad = user.AdSoyad;
				kullanici.Sifre = user.Sifre;
				kullanici.Telefon = user.Telefon;

				c.Entry(kullanici).State = EntityState.Modified;
				c.Update(kullanici);
				c.SaveChanges();
				TempData["BasariMesaji"] = "saveledim genc.";
			}

			return RedirectToAction("KullaniciGuncelleme");
		}






		public IActionResult RandevuSilme()
		{
			var randevular = c.Randevular
				.Include(x => x.Islem)          // Islem ilişkisini dahil et
												// Kullanici ilişkisini dahil et
				.Include(x => x.Calisan)        // Calisan ilişkisini dahil et
				.ToList();                      // Veritabanındaki tüm randevuları alıyoruz

			return View(randevular);            // View'a gönderiyoruz
		}
		public IActionResult RandevuSil(int id)
		{
			var randevu = c.Randevular.Find(id);
			if (randevu != null)
			{
				c.Randevular.Remove(randevu);
				c.SaveChanges();
				TempData["BasariMesaji"] = "Randevu silme başarılı.";
				return RedirectToAction("RandevuSilme");
			}
			TempData["BasariMesaji"] = "Randevu silme başarısız.";
			return RedirectToAction("RandevuSilme");
		}
	}
}
