using System.Text.Json.Serialization;

namespace Amega.BinanceService.Webapi
{
    public class ReceivedCurrency
    {
        [JsonPropertyName("e")]
        public string CurrencySource { get; set; }
        [JsonPropertyName("s")]
        public string Currency { get; set; }
        [JsonPropertyName("p")]
        public string Price { get; set; }
        [JsonIgnore]
        public string SourceMessage { get; set; }
    }
}
