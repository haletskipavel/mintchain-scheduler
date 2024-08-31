using System.Text.Json.Serialization;

namespace Airdrops.Mint.Scheduler.Models.Dtos
{
    public class Energy
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("freeze")]
        public bool Freeze { get; set; }

        [JsonPropertyName("uid")]
        public string[] Uid { get; set; }

        [JsonPropertyName("includes")]
        public string[] Includes { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }
}
