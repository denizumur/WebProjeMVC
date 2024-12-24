using berber.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace berber.Controllers
{
    public class RandevuController : Controller
    {
        Context c = new Context();

        // Randevu Listesi
        public IActionResult Index()
        {
            // Randevular, Islem, Kullanici ve Calisan verilerini dahil ediyoruz
            var randevular = c.Randevular
                //.Include(x => x.Islem)          // Islem ilişkisini dahil et
                                                // Kullanici ilişkisini dahil et
                .Include(x => x.Calisan)        // Calisan ilişkisini dahil et
                .ToList();                      // Veritabanındaki tüm randevuları alıyoruz

            return View(randevular);            // View'a gönderiyoruz
        }

        // Yeni Randevu Sayfası
        [HttpGet]
        public IActionResult YeniRandevu()
        {
            // Çalışanlar ve İşlemler için dropdown verilerini alıyoruz
            ViewBag.Calisanlar = c.Calisanlar.ToList(); // Çalışanlar listesi
            ViewBag.Islemler = c.Islemler.ToList(); // İşlemler listesi

            return View();
        }

        // Yeni Randevu Kaydetme (POST)
        [HttpPost]
        public IActionResult YeniRandevu(Randevu randevu)
        {
            if (ModelState.IsValid)
            {
                // Randevuyu veritabanına kaydediyoruz
                c.Randevular.Add(randevu);
                c.SaveChanges();

                // Kaydettikten sonra ana sayfaya yönlendiriyoruz
                return RedirectToAction("Index", "Randevu");
            }

            // Eğer model geçerli değilse, dropdown verileriyle birlikte tekrar formu gösteriyoruz
            ViewBag.Calisanlar = c.Calisanlar.ToList();
            ViewBag.Islemler = c.Islemler.ToList();
            return View(randevu);
        }


        // Randevu Detayı Görüntüleme
        public IActionResult RandevuGetir(int id)
        {
            var randevu = c.Randevular.Find(id); // Randevuyu id'ye göre buluyoruz
            return View(randevu); // Detay sayfasını gösteriyoruz
        }

        // Randevu Güncelleme
        [HttpGet]
        public IActionResult RandevuGuncelle(int id)
        {
            var randevu = c.Randevular.Find(id); // Güncellenecek randevuyu buluyoruz
            if (randevu == null)
            {
                return NotFound(); // Randevu bulunamazsa 404 döneriz
            }

            // Çalışanlar ve İşlemler için dropdown verilerini gönderiyoruz
            ViewBag.Calisanlar = c.Calisanlar.ToList();
            ViewBag.Islemler = c.Islemler.ToList();

            return View(randevu); // Güncelleme formunu gösteriyoruz
        }

        // Randevu Güncelleme (POST)
        [HttpPost]
        public IActionResult RandevuGuncelle(Randevu randevu)
        {
            if (ModelState.IsValid)
            {
                // Veritabanındaki randevuyu güncelliyoruz
                var existingRandevu = c.Randevular.Find(randevu.RandevuID);
                if (existingRandevu != null)
                {
                    //existingRandevu.MusteriAdi = randevu.MusteriAdi;
                    //existingRandevu.CalisanID = randevu.CalisanID;
                    //existingRandevu.IslemID = randevu.IslemID;
                    existingRandevu.Tarih = randevu.Tarih;
                    existingRandevu.Durum = randevu.Durum;
                    c.SaveChanges(); // Güncellemeyi kaydediyoruz
                }

                return RedirectToAction("Index"); // Liste sayfasına yönlendiriyoruz
            }

            // Eğer model geçerli değilse, dropdown verileriyle birlikte tekrar formu gösteriyoruz
            ViewBag.Calisanlar = c.Calisanlar.ToList();
            ViewBag.Islemler = c.Islemler.ToList();
            return View(randevu);
        }

        // Randevu Silme
        public IActionResult RandevuSil(int id)
        {
            var randevu = c.Randevular.Find(id); // Silinecek randevuyu buluyoruz
            if (randevu != null)
            {
                c.Randevular.Remove(randevu); // Randevuyu siliyoruz
                c.SaveChanges(); // Değişiklikleri kaydediyoruz
            }

            return RedirectToAction("Index"); // Liste sayfasına yönlendiriyoruz
        }
    }
}
