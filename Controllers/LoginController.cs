using berber.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.Drawing;
using System.Net.Http.Headers;

namespace berber.Controllers
{
	public class LoginController : Controller
	{
		Context c = new();

		private readonly ILogger<LoginController> _logger;
		private readonly IConfiguration _configuration;

		public LoginController(ILogger<LoginController> logger , IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		public IActionResult Index()
		{
			

			return View();
		}
		public IActionResult FotografYukleme()
		{


			return View();
		}


		public IActionResult Giris(Kullanici kullanici)
		{
			
			// Admin giriş kontrolü
			if ((kullanici.AdSoyad == "b231210350@sakarya.edu.tr" || kullanici.AdSoyad == "b221210038@sakarya.edu.tr") && kullanici.Sifre == "sau")
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












		[HttpGet]
		public IActionResult RandevuEkle()
		{
			if (HttpContext.Session.GetString("KullaniciID") is not null)
			{
				ViewBag.Calisanlar = c.Calisanlar.ToList();
				ViewBag.Islemler = c.Islemler.ToList();
				ViewBag.UygunSaatler = new List<TimeSpan>(); // Başlangıçta boş gönderiyoruz
				return View();
			}

			TempData["uyari"] = "Lütfen login olunuz.";
			return RedirectToAction("Index", "Login");
		}

		[HttpPost]
		public IActionResult RandevuEkle(Randevu model, List<int> selectedIslemler, string selectedSaat)
		{
			if (HttpContext.Session.GetString("KullaniciID") is null)
			{
				TempData["uyari"] = "Lütfen login olunuz.";
				return RedirectToAction("Index", "Login");
			}

			// Kullanıcı bilgisi session'dan alınıyor
			int? kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
			if (kullaniciID == null)
			{
				TempData["uyari"] = "Kullanıcı bilgisi bulunamadı!";
				return RedirectToAction("Index", "Login");
			}

			// Günlük randevu sayısı kontrolü
			var bugunkuRandevular = c.Randevular
				.Where(r => r.KullaniciID == kullaniciID && r.Tarih.Date == model.Tarih.Date)
				.Count();

			if (bugunkuRandevular >= 2)
			{
				TempData["HataMesaji"] = "Bir kullanıcı bir günde en fazla 2 randevu alabilir.";
				LoadViewData(model);
				return View(model);
			}

			// Model'in KullaniciID'sini giriş yapan kullanıcıya ata
			model.KullaniciID = kullaniciID;

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
				.ToList();

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

			if (randevuZamanlari.Any(r => selectedTime < r.Bitis && endTime > r.Baslangic))
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
			return RedirectToAction("Randevularim"); // Kullanıcıya özel bir sayfaya yönlendirme
		}







		private void LoadViewData(Randevu model)
		{
			ViewBag.Calisanlar = c.Calisanlar.ToList();
			ViewBag.Islemler = c.Islemler.ToList();

			var tumSaatler = new List<string>();
			for (var time = new TimeSpan(9, 0, 0); time < new TimeSpan(19, 0, 0); time = time.Add(TimeSpan.FromMinutes(10)))
			{
				tumSaatler.Add(time.ToString(@"hh\:mm"));
			}

			if (model.Tarih.Date == DateTime.Today)
			{
				var simdikiSaat = DateTime.Now.TimeOfDay;
				tumSaatler = tumSaatler.Where(saat =>
				{
					var timeSpan = TimeSpan.Parse(saat);
					return timeSpan > simdikiSaat;
				}).ToList();
			}

			ViewBag.UygunSaatler = tumSaatler;
		}


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











		//randevu görüntüleme

		public IActionResult Randevularim()
		{
			
			
			int? kullaniciID = HttpContext.Session.GetInt32("KullaniciID");

			if (kullaniciID == null)
			{
				TempData["uyari"] = "Lütfen giriş yapın.";
				return RedirectToAction("Index", "Login"); 
			}

			
			var randevular = c.Randevular
				.Where(r => r.KullaniciID == kullaniciID)  
				.Include(x => x.Calisan)         
				.Include(x => x.Kullanici)       
				.Include(x => x.RandevuIslemler) 
				.ThenInclude(ri => ri.Islem)     
				.ToList();                       

			return View(randevular);          
		}


		//randevu görüntüleme








		//session silme çıkış

		public IActionResult LogOut() {

			if (HttpContext.Session.GetString("KullaniciID") is not null)
			{

				HttpContext.Session.Clear();
				TempData["uyari"] = "Çıkış yaptınız";

			}


			return RedirectToAction("Index");
		}

		//session silme çıkış





		//kullanıcı bilgileri güncelleme başlangıç

		public IActionResult Guncelleme() {

			if (HttpContext.Session.GetString("KullaniciID") is not null)
			{
				int? kullaniciID = HttpContext.Session.GetInt32("KullaniciID");
				var kullanici = c.Kullanicilar.Find(kullaniciID);
				if (kullanici == null)
				{
					return RedirectToAction("Anasayfa");
				}
				return View(kullanici);
			}
			return RedirectToAction("Index", "Login");
		}
		
		[HttpGet]
		public IActionResult Guncelle(int id)
		{
			if (HttpContext.Session.GetString("KullaniciID") is not null)
			{

				var kullanici = c.Kullanicilar.Find(id);
				if (kullanici == null)
				{
					return RedirectToAction("Anasayfa");
				}
				return View(kullanici);
			}
			return RedirectToAction("Index", "Login");


		}

		[HttpPost]
		public IActionResult Guncelle(Kullanici user)
		{
			if (HttpContext.Session.GetString("KullaniciID") is not null)
			{
				int? kullaniciID = HttpContext.Session.GetInt32("KullaniciID");



				// Veritabanından Kullanıcıyı Getir

				var kullanici = c.Kullanicilar.FirstOrDefault(u => u.KullaniciID == kullaniciID);

				
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

				return RedirectToAction("Anasayfa");
			}
			return RedirectToAction("Index", "Login");



		}

		//kullanıcı bilgileri güncelleme bitiş























		[HttpPost]
		public async Task<IActionResult> FotografYukle(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				ModelState.AddModelError("", "Lütfen bir fotoðraf yükleyin.");
				return View("Index");
			}

			var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

			if (!Directory.Exists(uploadPath))
			{
				Directory.CreateDirectory(uploadPath);
			}

			var filePath = Path.Combine(uploadPath, file.FileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			var resizedFilePath = ResizeImage(filePath, 512, 512); // Dinamik boyutlandýrma
			var analizSonucu = await FotografAnalizEt(resizedFilePath);

			ViewBag.Analiz = analizSonucu;
			return View("FotografYukleme");
		}

		private string ResizeImage(string filePath, int width, int height)
		{
			using (var img = Image.FromFile(filePath))
			{
				var resized = new Bitmap(img, new Size(width, height));
				var resizedPath = Path.Combine(Path.GetDirectoryName(filePath), $"resized_{Path.GetFileName(filePath)}");

				var jpegEncoder = ImageCodecInfo.GetImageDecoders().First(c => c.FormatID == ImageFormat.Jpeg.Guid);
				var encoderParams = new EncoderParameters(1);
				encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 50L);  // %50 kalite

				resized.Save(resizedPath, jpegEncoder, encoderParams);
				return resizedPath;
			}
		}


		public IActionResult Hakkimizda() {


			if (HttpContext.Session.GetString("KullaniciID") is not null)
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





		private int TahminiTokenSayisi(string base64Image)
		{
			return (base64Image.Length * 3) / 4 / 1024; // Daha hassas token hesaplama
		}

		private async Task<string> FotografAnalizEt(string filePath)
		{
			var httpClient = new HttpClient();
			var imageBytes = System.IO.File.ReadAllBytes(filePath);
			var base64Image = Convert.ToBase64String(imageBytes);

			var apiKey = _configuration["GroqApiKey"];

			if (TahminiTokenSayisi(base64Image) > 7000)
			{
				return "Fotoðraf çok büyük, lütfen daha düþük çözünürlüklü bir fotoðraf yükleyin.";
			}

			var requestBody = new
			{
				model = "llama-3.2-90b-vision-preview",
				messages = new[]
				{
					new
					{
						role = "user",
						content = new object[]
						{
							new { type = "text", text = "Paylaþtýðým görselin detaylarýna göre analiz yaparak, yüz þekli, ten rengi ve tarzýnýza en uygun saç modeli veya rengi hakkýnda keskin ve net bir öneri sun." },
							new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64Image}" } }
						}
					}
				},
				max_tokens = 500
			};

			var json = JsonConvert.SerializeObject(requestBody);
			var stringContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

			httpClient.DefaultRequestHeaders.Authorization =
				new AuthenticationHeaderValue("Bearer", apiKey);

			var response = await httpClient.PostAsync("https://api.groq.com/openai/v1/chat/completions", stringContent);

			if (!response.IsSuccessStatusCode)
			{
				var hataMesaji = await response.Content.ReadAsStringAsync();
				return $"API hatasý: {hataMesaji}. Lütfen fotoðraf boyutunu kontrol edin veya daha düþük çözünürlüklü bir fotoðraf yükleyin.";
			}

			var result = await response.Content.ReadAsStringAsync();
			var jsonResponse = JsonConvert.DeserializeObject<dynamic>(result);
			return jsonResponse.choices[0].message.content.ToString();
		}





	}
}
