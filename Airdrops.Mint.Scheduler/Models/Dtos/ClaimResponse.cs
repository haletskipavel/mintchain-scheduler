using System.Text.Json.Serialization;

namespace Airdrops.Mint.Scheduler.Models.Dtos
{
    public class ClaimResponse
    {
        [JsonPropertyName("msg")]
        public string Msg { get; set; }
    }
}
