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
		Context c = new Context();

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
