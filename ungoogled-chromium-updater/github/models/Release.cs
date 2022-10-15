using System.Text.Json.Serialization;

namespace ungoogled_chromium_updater.github.models
{
    public class Release
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("assets")]

        public List<Asset> Assets { get; set; }
    }
}
