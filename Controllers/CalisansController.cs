using berber.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace berber.Controllers
{
    public class CalisanController : Controller
    {
        Context c = new Context();

        // Index method to list all employees (Calisans)
        public IActionResult Index()
        {
            var calisanlar = c.Calisanlar.ToList();
            return View(calisanlar);
        }

        // GET method to show the form for adding a new employee
        [HttpGet]
        public IActionResult YeniCalisan()
        {
            return View();
        }

        // POST method to save the new employee data
        [HttpPost]
        public IActionResult YeniCalisan(Calisan calisan)
        {
            if (ModelState.IsValid)
            {
                c.Calisanlar.Add(calisan);
                c.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(calisan);
        }

        // Method to delete an employee by their ID
        public IActionResult CalisanSil(int id)
        {
            var calisan = c.Calisanlar.Find(id);
            if (calisan != null)
            {
                c.Calisanlar.Remove(calisan);
                c.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // Method to get an employee by their ID for editing
        public IActionResult CalisanGetir(int id)
        {
            var calisan = c.Calisanlar.Find(id);
            if (calisan == null)
            {
                return NotFound();
            }
            return View(calisan);
        }

        // Method to update employee details
        [HttpPost]
        public IActionResult CalisanGuncelle(Calisan calisan)
        {
            if (ModelState.IsValid)
            {
                var existingCalisan = c.Calisanlar.Find(calisan.CalisanID);
                if (existingCalisan != null)
                {
                    existingCalisan.AdSoyad = calisan.AdSoyad;
                    existingCalisan.UzmanlikAlanlari = calisan.UzmanlikAlanlari;
                    existingCalisan.UygunlukSaatleri = calisan.UygunlukSaatleri;
                    c.SaveChanges();
                    return RedirectToAction("Index");
                }
                return NotFound();
            }
            return View(calisan);
        }
    }
}
