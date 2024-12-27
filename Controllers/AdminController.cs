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
				// Çalışanlar, hizmetler ve kullanıcılar ViewBag'e eklenir
				ViewBag.Calisanlar = c.Calisanlar.ToList();
				ViewBag.Islemler = c.Islemler.ToList();
				ViewBag.Kullanicilar = c.Kullanicilar.ToList();

				// Varsayılan olarak boş saat listesi gönderiyoruz
				ViewBag.UygunSaatler = new List<TimeSpan>();
				return View();
			}
			return RedirectToAction("Index", "Login");
		}

		// Tarihe ve çalışana göre uygun saatleri dinamik olarak hesapla
		[HttpGet]
		public IActionResult GetAvailableHours(int calisanId, DateTime selectedDate)
		{
			var tumSaatler = new List<string>();

			// 09:00 ile 17:00 arasında, 10 dakikalık aralıklarla saatleri oluştur
			for (var time = new TimeSpan(9, 0, 0); time < new TimeSpan(19, 0, 0); time = time.Add(TimeSpan.FromMinutes(10)))
			{
				tumSaatler.Add(time.ToString(@"hh\:mm"));
			}

			// Seçilen çalışanın belirtilen tarihte alınan randevuları ve hizmet sürelerini hesapla
			var alinanRandevular = c.Randevular
				.Where(r => r.CalisanID == calisanId && r.Tarih.Date == selectedDate.Date)
				.Select(r => new
				{
					BaslangicSaati = r.Tarih.TimeOfDay,
					RandevuID = r.RandevuID
				})
				.ToList();

			// Veritabanından Islemler tablosundaki tüm verileri çekin (bellek üzerinde işlem yapacağız)
			var islemler = c.Islemler.ToList();

			// Veritabanından RandevuIslemler tablosundaki tüm verileri çekin
			var randevuIslemler = c.RandevuIslemler.ToList();

			// Randevulara ait bitiş saatlerini hesapla
			var randevularlaBirlikte = alinanRandevular.Select(r =>
			{
				var toplamSure = randevuIslemler
					.Where(ri => ri.RandevuID == r.RandevuID)
					.Sum(ri =>
					{
						var islem = islemler.FirstOrDefault(i => i.IslemID == ri.IslemID);
						return islem?.Sure ?? 0;
					});

				// BitisSaati'ni hesapla ve anonim tipte döndür
				var bitisSaati = r.BaslangicSaati.Add(TimeSpan.FromMinutes(toplamSure));

				return new
				{
					r.BaslangicSaati,
					r.RandevuID,
					BitisSaati = bitisSaati
				};
			}).ToList();

			// Eğer tarih bugüne eşitse, geçmiş saatleri çıkar
			if (selectedDate.Date == DateTime.Today)
			{
				var currentTime = DateTime.Now.TimeOfDay;
				tumSaatler = tumSaatler.Where(saat =>
				{
					var saatSpan = TimeSpan.Parse(saat);
					return saatSpan > currentTime;
				}).ToList();
			}

			// Tüm saatlerden çakışan saatleri çıkar
			tumSaatler = tumSaatler.Where(saat =>
			{
				var saatSpan = TimeSpan.Parse(saat);

				// Çakışma kontrolü
				return !randevularlaBirlikte.Any(r =>
					saatSpan >= r.BaslangicSaati && saatSpan < r.BitisSaati
				);
			}).ToList();

			return Json(tumSaatler);
		}











		[HttpPost]
		public IActionResult RandevuEkle(Randevu model, List<int> selectedIslemler, string selectedSaat)
		{
			if (HttpContext.Session.GetString("admin") is not null)
			{
				// Tarih kontrolü
				if (model.Tarih.Date < DateTime.Today)
				{
					TempData["HataMesaji"] = "Bugünden önceki bir tarih için randevu oluşturulamaz.";
					LoadViewData(model);
					return View(model);
				}

				// Seçilen saat ile tarihi birleştir ve model.Tarih'e ata
				if (!string.IsNullOrEmpty(selectedSaat))
				{
					if (DateTime.TryParse($"{model.Tarih:yyyy-MM-dd} {selectedSaat}", out DateTime randevuZamani))
					{
						// Geçmiş saat kontrolü
						if (model.Tarih.Date == DateTime.Today && randevuZamani <= DateTime.Now)
						{
							TempData["HataMesaji"] = "Geçmiş bir saat için randevu oluşturulamaz.";
							LoadViewData(model);
							return View(model);
						}

						model.Tarih = randevuZamani;
					}
					else
					{
						TempData["HataMesaji"] = "Geçersiz saat seçimi.";
						LoadViewData(model);
						return View(model);
					}
				}
				else
				{
					TempData["HataMesaji"] = "Lütfen bir saat seçin.";
					LoadViewData(model);
					return View(model);
				}

				// İşlemleri ve sürelerini al
				var islemler = c.Islemler.ToList();
				var totalServiceTime = selectedIslemler
					.Select(islemID => islemler.FirstOrDefault(i => i.IslemID == islemID)?.Sure ?? 0)
					.Sum();

				// Randevu bitiş saatini hesapla
				TimeSpan selectedTime = model.Tarih.TimeOfDay;
				TimeSpan endTime = selectedTime.Add(TimeSpan.FromMinutes(totalServiceTime));

				// Kapanış saati kontrolü
				TimeSpan closingTime = new TimeSpan(19, 0, 0);
				if (endTime > closingTime)
				{
					TempData["HataMesaji"] = $"Seçilen saat ({selectedSaat}) ve işlemler toplam süresi kapanış saatini ({closingTime}) aşıyor.";
					LoadViewData(model);
					return View(model);
				}

				// Mevcut randevularla çakışma kontrolü
				var mevcutRandevular = c.Randevular
	.Where(r => r.CalisanID == model.CalisanID && r.Tarih.Date == model.Tarih.Date)
	.ToList(); // Sadece ilgili çalışanın randevuları getiriliyor

				var randevuZamanlari = mevcutRandevular.Select(r =>
				{
					var randevuBaslangic = r.Tarih.TimeOfDay;

					// Randevu sürelerini hesapla
					var randevuSure = c.RandevuIslemler
						.Where(ri => ri.RandevuID == r.RandevuID)
						.ToList() // Sadece bu randevuya ait işlemleri al
						.Sum(ri =>
						{
							var islem = islemler.FirstOrDefault(i => i.IslemID == ri.IslemID);
							return islem?.Sure ?? 0; // Süre null ise 0 döner
						});

					var randevuBitis = randevuBaslangic.Add(TimeSpan.FromMinutes(randevuSure));
					return new { Baslangic = randevuBaslangic, Bitis = randevuBitis };
				}).ToList();

				// Yeni randevunun başlangıç ve bitiş zamanlarını hesapla
				

				// Mevcut randevularla çakışma kontrolü
				if (randevuZamanlari.Any(r =>
					(selectedTime < r.Bitis && endTime > r.Baslangic) // Zaman çakışması
				))
				{
					TempData["HataMesaji"] = "Seçilen saat mevcut randevularla çakışıyor.";
					LoadViewData(model);
					return View(model);
				}

				// Randevuyu kaydet
				model.Durum = false;
				c.Randevular.Add(model);
				c.SaveChanges();

				// Seçilen işlemleri kaydet
				if (selectedIslemler != null && selectedIslemler.Any())
				{
					foreach (var islemID in selectedIslemler)
					{
						var randevuIslem = new RandevuIslem
						{
							RandevuID = model.RandevuID,
							IslemID = islemID
						};
						c.RandevuIslemler.Add(randevuIslem);
					}
					c.SaveChanges();
				}

				TempData["BasariMesaji"] = "Randevu başarıyla kaydedildi.";
				return RedirectToAction("RandevuGoruntuleme");
			}

			return RedirectToAction("Index", "Login");
		}





















		// Yardımcı metot: Gerekli ViewData'ları yükle
		private void LoadViewData(Randevu model)
		{
			ViewBag.Calisanlar = c.Calisanlar.ToList();
			ViewBag.Islemler = c.Islemler.ToList();
			ViewBag.Kullanicilar = c.Kullanicilar.ToList();

			// Tüm saatleri oluştur (09:00 ile 17:00 arası, 10 dakikalık aralıklarla)
			var tumSaatler = new List<string>();
			for (var time = new TimeSpan(9, 0, 0); time < new TimeSpan(19, 0, 0); time = time.Add(TimeSpan.FromMinutes(10)))
			{
				tumSaatler.Add(time.ToString(@"hh\:mm"));
			}

			// Daha önce alınan saatleri çıkar
			var alinanSaatler = c.Randevular
				.Where(r => r.Tarih.Date == model.Tarih.Date && r.CalisanID == model.CalisanID)
				.Select(r => r.Tarih.ToString("HH:mm"))
				.ToList();

			// Geçmiş saatleri de filtrele (sadece bugünkü tarih için)
			if (model.Tarih.Date == DateTime.Today)
			{
				var simdikiSaat = DateTime.Now.TimeOfDay;
				tumSaatler = tumSaatler.Where(saat =>
				{
					var timeSpan = TimeSpan.Parse(saat);
					return timeSpan > simdikiSaat;
				}).ToList();
			}

			// Uygun saatleri ViewBag'e aktar
			ViewBag.UygunSaatler = tumSaatler.Except(alinanSaatler).ToList();
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





























		public List<TimeSpan> GetAvailableHoursForCalisan(int calisanId, DateTime selectedDate)
		{
			// Çalışanın hizmet saatlerini belirleyin (örnek: 09:00 - 18:00)
			TimeSpan startHour = new TimeSpan(9, 0, 0); // 09:00
			TimeSpan endHour = new TimeSpan(18, 0, 0);  // 18:00
			List<TimeSpan> hizmetSaatleri = new List<TimeSpan>();

			// Saat dilimlerini listeye ekle (örnek: her saat başı)
			for (TimeSpan time = startHour; time < endHour; time = time.Add(TimeSpan.FromHours(1)))
			{
				hizmetSaatleri.Add(time);
			}

			
			
				// Seçilen tarih için çalışanın mevcut randevularını getir
				var mevcutRandevular = c.Randevular
					.Where(r => r.CalisanID == calisanId && r.Tarih.Date == selectedDate.Date)
					.Select(r => r.Tarih.TimeOfDay) // Sadece saat kısmını al
					.ToList();

				// Dolu saatleri hizmet saatlerinden çıkar
				var uygunSaatler = hizmetSaatleri
					.Where(saat => !mevcutRandevular.Contains(saat))
					.ToList();

				return uygunSaatler;
			
		}

		public IActionResult RandevuAl(int calisanId, DateTime selectedDate)
		{
			var uygunSaatler = GetAvailableHoursForCalisan(calisanId, selectedDate);

			// Saatleri View'e model olarak gönderiyoruz
			ViewBag.UygunSaatler = uygunSaatler;
			return View();
		}






	}
}
