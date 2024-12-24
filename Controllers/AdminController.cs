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
			if (HttpContext.Session.GetString("admin") is not null)
			{
				return View();
			}
			return RedirectToAction("Index","Login");
		}
		public IActionResult LogOut()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{

				HttpContext.Session.Clear();
				TempData["uyari"] = "Çıkış yaptınız";

			}


			return RedirectToAction("Index","Login");
		}

		public IActionResult CalisanIslemleri()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				return View();
			}
			return RedirectToAction("Index", "Login");
		}
		public IActionResult HizmetIslemleri()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				return View();
			}
			return RedirectToAction("Index", "Login");
		}
		public IActionResult RandevuIslemleri()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				return View();
			}
			return RedirectToAction("Index", "Login");
		}
		public IActionResult KullaniciIslemleri()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				return View();
			}
			return RedirectToAction("Index", "Login");
		}




		public IActionResult CalisanGoruntuleme()
		{

			if (HttpContext.Session.GetString("admin") is not null)
			{
				// Çalışanları ve her bir çalışanın ilgili işlemlerini (hizmetlerini) eager loading ile alıyoruz.
				var calisanlar = c.Calisanlar
					.Include(c => c.CalisanIslemler)  // Çalışan işlemleri
					.ThenInclude(ci => ci.Islem)     // Her işlemle ilişkili olan işlem (hizmet) bilgisi
					.ToList();

				return View(calisanlar);
			}
			return RedirectToAction("Index", "Login");
			
		}
		[HttpGet]
		public IActionResult CalisanEkle()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				return View();
			}
			return RedirectToAction("Index", "Login");


		}

		[HttpPost]
		public IActionResult CalisanEkle(Calisan calisan)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var mevcutCalisan = c.Calisanlar.FirstOrDefault(x => x.AdSoyad == calisan.AdSoyad && x.UygunlukSaatleri == calisan.UygunlukSaatleri);

				if (mevcutCalisan != null)
				{
					TempData["HataMesaji"] = "Bu çalışan daha önce tanımlanmış.";
					ModelState.Clear();
					return View("CalisanEkle");
				}

				c.Calisanlar.Add(calisan);
				c.SaveChanges(); // Çalışanı kaydediyoruz
				return RedirectToAction("CalisanGoruntuleme");
			}
			return RedirectToAction("Index", "Login");


		}


		public IActionResult CalisanSilme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var calisanlar = c.Calisanlar.ToList();
				return View(calisanlar);
			}
			return RedirectToAction("Index", "Login");


		}
		public IActionResult CalisanSil(int id)
		{
			if (HttpContext.Session.GetString("admin") is not null)
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
			return RedirectToAction("Index", "Login");


		}
		public IActionResult CalisanGuncelleme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var calisanlar = c.Calisanlar.ToList();
				return View(calisanlar);
			}
			return RedirectToAction("Index", "Login");


		}

		[HttpGet]
		public IActionResult CalisanGuncelle(int id)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var calisan = c.Calisanlar.Find(id);
				if (calisan == null)
				{
					return RedirectToAction("CalisanGuncelleme");
				}
				return View(calisan);
			}
			return RedirectToAction("Index", "Login");


		}

		[HttpPost]
		public IActionResult CalisanGuncelle(Calisan cal)
		{

			if (HttpContext.Session.GetString("admin") is not null)
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
			return RedirectToAction("Index", "Login");


		}









		public IActionResult HizmetGoruntuleme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var degerler = c.Islemler
				.Include(i => i.CalisanIslemler)  // Hizmetin ilişkili olduğu çalışanları yükle
				.ThenInclude(ci => ci.Calisan)   // Her çalışanı yükle
				.ToList();

				return View(degerler);
			}
			return RedirectToAction("Index", "Login");

			
		}
		[HttpGet]
		public IActionResult HizmetEkle()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				ViewBag.Calisanlar = c.Calisanlar.ToList(); // Çalışanları View'a gönderiyoruz
				return View();
			}
			return RedirectToAction("Index", "Login");


		}

		[HttpPost]
		public IActionResult HizmetEkle(Islem islem, List<int> secilenCalisanlar)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				// Mevcut hizmeti kontrol et
				var mevcutIslem = c.Islemler.FirstOrDefault(x => x.IslemAdi == islem.IslemAdi && x.Ucret == islem.Ucret);
				if (mevcutIslem != null)
				{
					TempData["HataMesaji"] = "Bu hizmet daha önce tanımlanmış.";
					ModelState.Clear();
					return View("HizmetEkle");
				}

				// Yeni hizmeti ekle
				c.Islemler.Add(islem);
				c.SaveChanges(); // Hizmeti kaydet

				// Seçilen çalışanlarla ilişkiyi kur
				if (secilenCalisanlar != null && secilenCalisanlar.Count > 0)
				{
					foreach (var calisanId in secilenCalisanlar)
					{
						var calisanIslem = new CalisanIslem
						{
							IslemID = islem.IslemID,
							CalisanID = calisanId
						};
						c.CalisanIslemler.Add(calisanIslem); // Çalışan ile hizmet ilişkilendiriliyor
					}

					c.SaveChanges(); // İlişkileri kaydet
				}

				return RedirectToAction("HizmetGoruntuleme");
			}
			return RedirectToAction("Index", "Login");



		}


		public IActionResult HizmetSilme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var degerler = c.Islemler
				.Include(i => i.CalisanIslemler)  // Hizmetin ilişkili olduğu çalışanları yükle
				.ThenInclude(ci => ci.Calisan)   // Her çalışanı yükle
				.ToList();

				return View(degerler);
			}
			return RedirectToAction("Index", "Login");


		}

		public IActionResult HizmetSil(int id)
		{
			if (HttpContext.Session.GetString("admin") is not null)
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
			return RedirectToAction("Index", "Login");


		}

		public IActionResult HizmetGuncelleme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var degerler = c.Islemler
				.Include(i => i.CalisanIslemler)  // Hizmetin ilişkili olduğu çalışanları yükle
				.ThenInclude(ci => ci.Calisan)   // Her çalışanı yükle
				.ToList();

				return View(degerler);
			}
			return RedirectToAction("Index", "Login");


		}

		[HttpGet]
		public IActionResult HizmetGuncelle(int id)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				// Belirtilen hizmeti ve ilişkili çalışanları getir
				var hizmet = c.Islemler.Include(x => x.CalisanIslemler).FirstOrDefault(x => x.IslemID == id);
				if (hizmet == null)
				{
					TempData["HataMesaji"] = "Hizmet bulunamadı.";
					return RedirectToAction("HizmetGuncelleme");
				}

				// ViewBag ile çalışanlar ve seçili çalışanları gönder
				ViewBag.Calisanlar = c.Calisanlar.ToList();
				ViewBag.SeciliCalisanlar = hizmet.CalisanIslemler.Select(ci => ci.CalisanID).ToList();

				return View(hizmet); // Tek bir Islem nesnesi gönder
			}

			return RedirectToAction("Index", "Login");
		}

		[HttpPost]
		public IActionResult HizmetGuncelle(Islem islem, List<int> secilenCalisanlar)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var hizmet = c.Islemler.Include(x => x.CalisanIslemler).FirstOrDefault(x => x.IslemID == islem.IslemID);
				if (hizmet != null)
				{
					// Hizmet bilgilerini güncelle
					hizmet.IslemAdi = islem.IslemAdi;
					hizmet.Sure = islem.Sure;
					hizmet.Ucret = islem.Ucret;

					// Mevcut çalışan ilişkilerini temizle
					c.CalisanIslemler.RemoveRange(hizmet.CalisanIslemler);

					// Yeni seçilen çalışanları ekle
					if (secilenCalisanlar != null && secilenCalisanlar.Any())
					{
						foreach (var calisanID in secilenCalisanlar)
						{
							hizmet.CalisanIslemler.Add(new CalisanIslem { CalisanID = calisanID, IslemID = hizmet.IslemID });
						}
					}

					c.Entry(hizmet).State = EntityState.Modified;
					c.SaveChanges();

					TempData["BasariMesaji"] = "Hizmet başarıyla güncellendi.";
					return RedirectToAction("HizmetGuncelleme");
				}

				TempData["HataMesaji"] = "Hizmet bulunamadı.";
				return RedirectToAction("HizmetGuncelleme");
			}

			return RedirectToAction("Index", "Login");
		}









		public IActionResult KullaniciGoruntuleme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var kullanicilar = c.Kullanicilar.ToList();
				return View(kullanicilar);
			}
			return RedirectToAction("Index", "Login");

		
		}

		[HttpGet]
		public IActionResult KullaniciEkle()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				return View();
			}
			return RedirectToAction("Index", "Login");

		
		}

		[HttpPost]
		public IActionResult KullaniciEkle(Kullanici user)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{


				var mevcutIslem = c.Kullanicilar.FirstOrDefault(x => x.AdSoyad == user.AdSoyad && x.Sifre == user.Sifre);

				if (mevcutIslem != null)
				{
					// Eğer varsa hata mesajı göster
					TempData["HataMesaji"] = user.AdSoyad + " isimli kullanıcı daha önce tanımlanmış.";

					// ModelState'i temizle, böylece form verisi sıfırlanır ve kullanıcıyı tekrar girmesi için gösteririz
					ModelState.Clear();
					return View("KullaniciEkle");
				}

				// Kullanıcıyı kaydetmeden önce UyelikTarihi'ni atayalım
				user.UyelikTarihi = DateTime.Now;

				// Yeni kullanıcıyı veritabanına ekle
				c.Kullanicilar.Add(user);
				c.SaveChanges();

				// Başarılı işlem mesajı
				TempData["BasariMesaji"] = "Kullanıcı başarıyla eklendi.";
				return RedirectToAction("KullaniciGoruntuleme");


			}
			return RedirectToAction("Index", "Login");
		
		}


		public IActionResult KullaniciSilme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var kullanicilar = c.Kullanicilar.ToList();
				return View(kullanicilar);
			}
			return RedirectToAction("Index", "Login");

		
		}

		public IActionResult KullaniciSil(int id)
		{
			if (HttpContext.Session.GetString("admin") is not null)
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
			return RedirectToAction("Index", "Login");

			
		}

		public IActionResult KullaniciGuncelleme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var kullanicilar = c.Kullanicilar.ToList();
				return View(kullanicilar);
			}
			return RedirectToAction("Index", "Login");

		
		}

		[HttpGet]
		public IActionResult KullaniciGuncelle(int id)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{

				var kullanici = c.Kullanicilar.Find(id);
				if (kullanici == null)
				{
					return RedirectToAction("KullaniciGuncelleme");
				}
				return View(kullanici);
			}
			return RedirectToAction("Index", "Login");


		}

		[HttpPost]
		public IActionResult KullaniciGuncelle(Kullanici user)
		{
			if (HttpContext.Session.GetString("admin") is not null)
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
			return RedirectToAction("Index", "Login");


			
		}


		
		public IActionResult RandevuGoruntuleme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var randevular = c.Randevular
				.Include(x => x.Calisan)         // Calisan ilişkisini dahil et
				.Include(x => x.Kullanici)       // Kullanici ilişkisini dahil et
				.Include(x => x.RandevuIslemler) // RandevuIslemler ilişkisini dahil et
					.ThenInclude(ri => ri.Islem) // RandevuIslem üzerinden Islem ilişkisini dahil et
				.ToList();                       // Veritabanındaki tüm randevuları alıyoruz

				return View(randevular);            // Randevuları View'a gönderiyoruz
			}
			return RedirectToAction("Index", "Login");


		}

		public IActionResult RandevuSilme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var randevular = c.Randevular
				.Include(x => x.Calisan)         // Calisan ilişkisini dahil et
				.Include(x => x.Kullanici)       // Kullanici ilişkisini dahil et
				.Include(x => x.RandevuIslemler) // RandevuIslemler ilişkisini dahil et
					.ThenInclude(ri => ri.Islem) // RandevuIslem üzerinden Islem ilişkisini dahil et
				.ToList();                       // Veritabanındaki tüm randevuları alıyoruz

				return View(randevular);            // Randevuları View'a gönderiyoruz
			}
			return RedirectToAction("Index", "Login");

		
		}

		public IActionResult RandevuSil(int id)
		{
			if (HttpContext.Session.GetString("admin") is not null)
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
			return RedirectToAction("Index", "Login");

			
		}



		[HttpGet]
		public IActionResult RandevuEkle()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				ViewBag.Calisanlar = c.Calisanlar.ToList();  // Çalışanlar
				ViewBag.Islemler = c.Islemler.ToList();      // Hizmetler
				ViewBag.Kullanicilar = c.Kullanicilar.ToList();  // Kullanıcılar
				return View();
			}
			return RedirectToAction("Index", "Login");

			
		}


		[HttpPost]
		public IActionResult RandevuEkle(Randevu model, List<int> selectedIslemler)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				// Eğer model geçerli değilse, hata mesajı göster
				if (!ModelState.IsValid)
				{
					TempData["HataMesaji"] = "Lütfen tüm alanları doğru doldurun.";
					ViewBag.Calisanlar = c.Calisanlar.ToList();
					ViewBag.Islemler = c.Islemler.ToList();
					ViewBag.Kullanicilar = c.Kullanicilar.ToList();
					return View(model);
				}

				// Kullanıcıyı geçici olarak atama kısmı kaldırıldı.
				// Modelde gönderilen KullaniciID'yi alıyoruz
				// Eğer modelin KullaniciID'si null ise, geçici bir kullanıcı id'si atanabilir (bu kısmı özelleştirebilirsiniz)
				if (model.KullaniciID == null)
				{
					model.KullaniciID = 2; // Burada bir kullanıcı ID'si belirleyebilirsiniz, örneğin admin.
				}

				// Durum alanı null ise varsayılan olarak false (beklemede) kabul edelim
				if (model.Durum == null)
				{
					model.Durum = false;
				}

				// Randevuyu kaydet
				c.Randevular.Add(model);
				c.SaveChanges(); // Kaydetme işlemi

				// Seçilen hizmetleri RandevuIslem tablosuna ekle
				if (selectedIslemler != null && selectedIslemler.Any())
				{
					foreach (var islemID in selectedIslemler)
					{
						var randevuIslem = new RandevuIslem
						{
							RandevuID = model.RandevuID,  // Yeni kaydedilen randevu ID'si
							IslemID = islemID
						};
						c.RandevuIslemler.Add(randevuIslem);
					}

					// Hizmet kayıtlarını kaydet
					c.SaveChanges();
				}

				TempData["BasariMesaji"] = "Randevu başarıyla kaydedildi.";
				return RedirectToAction("RandevuGoruntuleme");
			}
			return RedirectToAction("Index", "Login");

		
		}
		public IActionResult RandevuGuncelleme()
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				var randevular = c.Randevular
				.Include(x => x.Calisan)         // Calisan ilişkisini dahil et
				.Include(x => x.Kullanici)       // Kullanici ilişkisini dahil et
				.Include(x => x.RandevuIslemler) // RandevuIslemler ilişkisini dahil et
					.ThenInclude(ri => ri.Islem) // RandevuIslem üzerinden Islem ilişkisini dahil et
				.ToList();                       // Veritabanındaki tüm randevuları alıyoruz

				return View(randevular);            // Randevuları View'a gönderiyoruz
			}
			return RedirectToAction("Index", "Login");


		}

		[HttpGet]
		public IActionResult RandevuGuncelle(int id)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{

				// Randevuyu ID'ye göre al
				var randevu = c.Randevular
								.Include(r => r.Calisan)
								.FirstOrDefault(r => r.RandevuID == id);

				if (randevu == null)
				{
					return NotFound();  // Eğer randevu bulunamazsa 404 döndür
				}

				// Calisanlar'ı ViewBag'e gönder
				ViewBag.Calisanlar = c.Calisanlar.ToList();
				ViewBag.Kullanicilar = c.Kullanicilar.ToList();
				ViewBag.Islemler = c.Islemler.ToList(); // Eğer hizmetleri de listelemeniz gerekiyorsa

				return View(randevu);


			}
			return RedirectToAction("Index", "Login");



		}


		[HttpPost]
		public IActionResult RandevuGuncelle(int id, Randevu model)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				if (!ModelState.IsValid)
				{
					TempData["HataMesaji"] = "Lütfen tüm alanları doğru doldurun.";
					return View(model);
				}

				// Randevuyu ID'ye göre al
				var randevu = c.Randevular.FirstOrDefault(r => r.RandevuID == id);

				if (randevu == null)
				{
					return NotFound();  // Eğer randevu bulunamazsa 404 döndür
				}

				// Randevu bilgilerini güncelle
				randevu.Tarih = model.Tarih;
				randevu.Durum = model.Durum;  // Durum true olarak güncellenir
				randevu.CalisanID = model.CalisanID;
				randevu.KullaniciID = model.KullaniciID;

				// Veritabanında güncellemeyi kaydet
				c.SaveChanges();

				TempData["BasariMesaji"] = "Randevu başarıyla güncellendi.";
				return RedirectToAction("RandevuGoruntuleme");  // Güncellenmiş randevuları görüntüle
			}
			return RedirectToAction("Index", "Login");


		}






		[HttpGet]
		public IActionResult GetIslemlerByCalisan(int calisanId)
		{
			var islemler = c.Islemler
				.Where(islem => islem.CalisanIslemler.Any(ci => ci.CalisanID == calisanId))
				.Select(islem => new
				{
					islemID = islem.IslemID,
					islemAdi = islem.IslemAdi
				})
				.ToList();

			return Json(islemler);
		}




		[HttpPost]
		public IActionResult HizmettenCalisanCikar(int hizmetID, int calisanID)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				// Hizmet ve Çalışan İlişkisini Kontrol Et
				var calisanHizmet = c.CalisanIslemler.FirstOrDefault(ci => ci.IslemID == hizmetID && ci.CalisanID == calisanID);
				if (calisanHizmet != null)
				{
					// Randevuların Etkilenmemesi İçin Kontrol
					var etkilenmisRandevular = c.RandevuIslemler
						.Include(ri => ri.Randevu)
						.Where(ri => ri.IslemID == hizmetID && ri.Randevu.CalisanID == calisanID)
						.ToList();

					if (etkilenmisRandevular.Any())
					{
						// Randevuları İşleme Al
						foreach (var randevuIslem in etkilenmisRandevular)
						{
							// Randevuda başka bir çalışan ataması yapılabilir ya da uyarı verilebilir
							//randevuIslem.CalisanID = null; // İsteğe bağlı olarak boş bırakabiliriz
						}

						c.SaveChanges();
					}

					// Hizmetten Çalışanı Sil
					c.CalisanIslemler.Remove(calisanHizmet);
					c.SaveChanges();

					TempData["BasariMesaji"] = "Çalışan hizmetten başarıyla çıkarıldı.";
				}
				else
				{
					TempData["HataMesaji"] = "Belirtilen çalışan hizmette bulunamadı.";
				}

				return RedirectToAction("HizmetGuncelle", new { id = hizmetID });
			}

			return RedirectToAction("Index", "Login");
		}






	}
}
