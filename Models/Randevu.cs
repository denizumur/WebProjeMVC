using System.ComponentModel.DataAnnotations;

namespace berber.Models
{
    public class Randevu
    {
        [Key]
        public int RandevuID { get; set; }  // AppointmentID
        public DateTime Tarih { get; set; }  // Tarih
        public Boolean? Durum { get; set ;}   // Durum (Onaylı, Beklemede vs.)
 
		public ICollection<RandevuIslem>? RandevuIslemler { get; set; }

		public int? KullaniciID { get; set; } // Foreign Key
		public Kullanici? Kullanici { get; set; }

		public int? CalisanID { get; set; } //foreign key
		public Calisan? Calisan { get; set; }
	}

}
