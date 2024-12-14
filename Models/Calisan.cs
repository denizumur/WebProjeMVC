using System.ComponentModel.DataAnnotations;

namespace berber.Models
{
    public class Calisan
    {
        [Key]
        public int CalisanID { get; set; }  // EmployeeID
        public string AdSoyad { get; set; }  // AdSoyad
        public string UzmanlikAlanlari { get; set; }  // UzmanlikAlanlari
        public string UygunlukSaatleri { get; set; }  // UygunlukSaatleri

       
    }
}