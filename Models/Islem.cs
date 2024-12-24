using System.ComponentModel.DataAnnotations;

namespace berber.Models
{
    public class Islem
    {
        [Key]
        public int IslemID { get; set; }  // ServiceID

        public string IslemAdi { get; set; }  // IslemAdi
        public int Sure { get; set; }  // Sure
        public int Ucret { get; set; }  // Ucret

		public ICollection<CalisanIslem> CalisanIslemler { get; set; }

		public ICollection<RandevuIslem> RandevuIslemler { get; set; }
	}

}
