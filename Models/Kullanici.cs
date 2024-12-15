using System.ComponentModel.DataAnnotations;

namespace berber.Models
{
    public class Kullanici
    {
        [Key]
        public int KullaniciID { get; set; }  // UserID

        [Display (Name = "Kullanıcı Adı")]
        public string AdSoyad { get; set; }  // AdSoyad

		[Display(Name = "Kullanıcı Üyelik Tarihi")]
		public DateTime UyelikTarihi { get; set; }  // Tarih

		[Display(Name = "Kullanıcı Telefon Numarası")]
		public string Telefon { get; set; }  // Telefon

		[Display(Name = "Kullanıcı Şifresi")]
		public string Sifre { get; set; }  // Sifre

        // İlişkiler
       
    }

}
