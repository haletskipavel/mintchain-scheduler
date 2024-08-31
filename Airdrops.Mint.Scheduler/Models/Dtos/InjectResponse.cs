using System.Text.Json.Serialization;

namespace Airdrops.Mint.Scheduler.Models.Dtos
{
    public class InjectResponse
    {
        [JsonPropertyName("msg")]
        public string Message { get; set; }
    }
}
