using System.ComponentModel.DataAnnotations;

namespace berber.Models
{
	public class CalisanIslem
	{
		[Key]
		public int CalisanIslemID { get; set; }

		public int CalisanID { get; set; }
		public Calisan Calisan { get; set; }

		public int IslemID { get; set; }
		public Islem Islem { get; set; }
	}
}
