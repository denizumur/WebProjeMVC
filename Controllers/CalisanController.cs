using berber.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
          
                c.Calisanlar.Add(calisan);
                c.SaveChanges();
                return RedirectToAction("Index");
            
            
        }

        // Method to delete an employee by their ID
        public IActionResult CalisanSil(int id)
        {
            var calisan = c.Calisanlar.Find(id);
            if (calisan != null)
            {
                c.Calisanlar.Remove(calisan);
                c.SaveChanges();
                return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }

    }
}
