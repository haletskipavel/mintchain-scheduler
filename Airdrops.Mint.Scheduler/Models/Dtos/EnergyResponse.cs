using System.Text.Json.Serialization;

namespace Airdrops.Mint.Scheduler.Models.Dtos
{
    public class EnergyResponse
    {
        [JsonPropertyName("result")]
        public List<Energy> Result { get; set; }
    }
}
