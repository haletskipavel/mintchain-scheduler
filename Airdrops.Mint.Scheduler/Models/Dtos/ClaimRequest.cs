using System.Text.Json.Serialization;

namespace Airdrops.Mint.Scheduler.Models.Dtos
{
    public class ClaimRequest
    {
        [JsonPropertyName("uid")]
        public string[] Uid { get; set; }

        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("includes")]
        public string[] Includes { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("freeze")]
        public bool Freeze { get; set; } = false;
    }
}
