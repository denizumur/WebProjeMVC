using System.ComponentModel.DataAnnotations;

namespace berber.Models
{
    public class Calisan
    {
        [Key]
        public int CalisanID { get; set; }  
        [Required(ErrorMessage = "Ad ve Soyad gereklidir.")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Uzmanlık Alanları gereklidir.")]
        public string UzmanlikAlanlari { get; set; }

        [Required(ErrorMessage = "Uygunluk Saatleri gereklidir.")]
        public required string UygunlukSaatleri { get; set; }
        public ICollection<Randevu> Randevular { get; set; }
    }
}