using System.ComponentModel.DataAnnotations;

namespace berber.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciID { get; set; }  // UserID
        public string AdSoyad { get; set; }  // AdSoyad
        public DateTime UyelikTarihi { get; set; }  // Email
        public string Telefon { get; set; }  // Telefon
        public string Sifre { get; set; }  // Sifre

        // İlişkiler
       
    }

}
