using System.ComponentModel.DataAnnotations;

namespace berber.Models
{
    public class Randevu
    {
        [Key]
        public int RandevuID { get; set; }  // AppointmentID
        public int CalisanID { get; set; }  // EmployeeID (FK)
        public int IslemID { get; set; }  // ServiceID (FK)
        public string MusteriAdi { get; set; }  // MusteriAdi
        public DateTime Tarih { get; set; }  // Tarih
        public string Durum { get; set; }  // Durum (Onaylı, Beklemede vs.)

        // İlişkiler
        public Calisan? Calisan { get; set; }  // Çalışan
        public Islem? Islem { get; set; }  // İşlem
		public Kullanici? Kullanici { get; set; }  // İşlem
		public int? KullaniciID { get; set; }  // ServiceID (FK)

	}

}
