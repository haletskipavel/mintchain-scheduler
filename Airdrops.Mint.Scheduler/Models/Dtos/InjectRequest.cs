using System.Text.Json.Serialization;

namespace Airdrops.Mint.Scheduler.Models.Dtos
{
    public class InjectRequest
    {
        [JsonPropertyName("energy")]
        public int Energy { get; set; }

        [JsonPropertyName("address")]
        public string Address { get; set; }
    }
}
