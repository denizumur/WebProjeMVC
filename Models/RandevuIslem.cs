using System.ComponentModel.DataAnnotations;

namespace berber.Models
{
	public class RandevuIslem
	{
		[Key]
		public int RandevuIslemID { get; set; }

		public int RandevuID { get; set; }
		public Randevu Randevu { get; set; }

		public int IslemID { get; set; }
		public Islem Islem { get; set; }

		
	}
}
