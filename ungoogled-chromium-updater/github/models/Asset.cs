using System.Text.Json.Serialization;

namespace ungoogled_chromium_updater.github.models
{
    public class Asset
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("browser_download_url")]
        public string DownloadUrl { get; set; }
    }
}
