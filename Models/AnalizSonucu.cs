using Newtonsoft.Json;

namespace berber.Models
{
	public class AnalizSonucu
	{
		[JsonProperty("data")]
		public List<AnalizDetay> Data { get; set; }
	}

	public class AnalizDetay
	{
		[JsonProperty("url")]
		public string Url { get; set; }
	}
}
