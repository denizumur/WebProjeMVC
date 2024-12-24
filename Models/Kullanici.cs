using System.ComponentModel.DataAnnotations;

namespace berber.Models
{
    public class Kullanici
    {
        [Key]
        public int? KullaniciID { get; set; }  // UserID


		[Required(ErrorMessage = "Lütfen ad alanını doldurunuz.")]
		[Display (Name = "Kullanıcı Adı")]
        public string AdSoyad { get; set; }  // AdSoyad




		[Display(Name = "Kullanıcı Üyelik Tarihi")]
		public DateTime? UyelikTarihi { get; set; }  // Tarih



		[Required(ErrorMessage = "Lütfen telefon alanını doldurunuz.")]
		[RegularExpression(@"^05\d{2}\d{3}\d{2}\d{2}$", ErrorMessage = "Telefon numaranızı doğru giriniz.")]
		[Display(Name = "Kullanıcı Telefon Numarası")]
		public string Telefon { get; set; }  // Telefon


        public ICollection<Randevu>? Randevular { get; set; }


        [Required(ErrorMessage="Lütfen şifre alanını doldurunuz.")]
		[Display(Name = "Kullanıcı Şifresi")]
		public string Sifre { get; set; }  // Sifre

        
       
    }

}
