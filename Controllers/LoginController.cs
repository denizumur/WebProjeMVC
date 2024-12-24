using berber.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.EntityFrameworkCore;

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
			
			// Admin giriş kontrolü
			if ((kullanici.AdSoyad == "melih" || kullanici.AdSoyad == "deniz") && kullanici.Sifre == "123")
			{
				HttpContext.Session.SetString("admin", "admins");
				return View("~/Views/Admin/Index.cshtml");
			}

			// Kullanıcıyı veritabanında kontrol et
			var mevcutIslem = c.Kullanicilar.FirstOrDefault(x => x.AdSoyad == kullanici.AdSoyad && x.Sifre == kullanici.Sifre);

			if (mevcutIslem != null)
			{
				// Kullanıcı bulundu, session'a kullanıcı ID'sini kaydet
				HttpContext.Session.SetInt32("KullaniciID", (int)mevcutIslem.KullaniciID); // ID'yi session'a kaydediyoruz

				TempData["uyari"] = "Başarılı giriş";
				return RedirectToAction("Anasayfa"); // Anasayfa'ya yönlendir
			}

			// Kullanıcı bulunamazsa
			TempData["uyari"] = "Başarısız giriş. Lütfen kullanıcı adı ve şifrenizi kontrol edin.";
			return RedirectToAction("Index"); // Giriş sayfasına tekrar yönlendir
		}
		public IActionResult Anasayfa()
		{

			if (HttpContext.Session.GetString("KullaniciID") is not null)
			{				
				return View();
			}

			TempData["uyari"] = "Lütfen login olunuz";
			return RedirectToAction("Index");
		}
		public IActionResult RandevuEkle() {

			if (HttpContext.Session.GetString("KullaniciID") is not null)
			{
				ViewBag.Calisanlar = c.Calisanlar.ToList();
				ViewBag.Islemler = c.Islemler.ToList();
				return View();
			}
			
			TempData["uyari"] = "Lütfen login olunuz";
			return RedirectToAction("Index");
		}

		[HttpPost]
		public IActionResult RandevuYeni(Randevu model, List<int> selectedIslemler)
		{
			// Kullanıcı girişi kontrolü
			if (HttpContext.Session.GetString("KullaniciID") == null)
			{
				TempData["uyari"] = "Lütfen login olunuz.";
				return RedirectToAction("Index", "Login"); // Login sayfasına yönlendir
			}

			// Kullanıcı bilgisini session'dan al
			int? kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
			if (kullaniciID == null)
			{
				TempData["uyari"] = "Kullanıcı bilgisi bulunamadı!";
				return RedirectToAction("Index", "Login");
			}

			// Model'in KullaniciID'sini giriş yapan kullanıcının ID'si ile güncelle
			model.KullaniciID = kullaniciID;

			// Durum alanını varsayılan olarak "Beklemede" (false) yap
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
			return RedirectToAction("Anasayfa"); // Randevuları görüntülemek için yönlendir
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


		public IActionResult Randevularim()
		{
			// Kullanıcı ID'sini session'dan al
			
			int? kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

			if (kullaniciID == null)
			{
				TempData["uyari"] = "Lütfen giriş yapın.";
				return RedirectToAction("Index", "Login"); // Giriş sayfasına yönlendir
			}

			// Kullanıcıya ait randevuları filtreleyerek sorgula
			var randevular = c.Randevular
				.Where(r => r.KullaniciID == kullaniciID)  // Sadece giriş yapan kullanıcının randevuları
				.Include(x => x.Calisan)         // Calisan ilişkisini dahil et
				.Include(x => x.Kullanici)       // Kullanici ilişkisini dahil et
				.Include(x => x.RandevuIslemler) // RandevuIslemler ilişkisini dahil et
				.ThenInclude(ri => ri.Islem)     // RandevuIslem üzerinden Islem ilişkisini dahil et
				.ToList();                       // Veritabanındaki tüm randevuları alıyoruz

			return View(randevular);            // Randevuları View'a gönderiyoruz
		}

		public IActionResult LogOut() {

			if (HttpContext.Session.GetString("KullaniciID") is not null)
			{

				HttpContext.Session.Clear();
				TempData["uyari"] = "Çıkış yaptınız";

			}


			return RedirectToAction("Index");
		}

	}
}
