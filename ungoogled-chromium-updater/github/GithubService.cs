using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;
using ungoogled_chromium_updater.github.models;

namespace ungoogled_chromium_updater.github
{
    internal class GithubService
    {
        private static readonly string RELEASES_URL = "https://api.github.com/repos/ungoogled-software/ungoogled-chromium-windows/releases";
        private static readonly Regex X64_BIT_REGEX = new("_installer_x64\\.exe");
        private static readonly Regex X86_BIT_REGEX = new("_installer_x86\\.exe");
        private static readonly Regex INSTALLER_REGEX = new("_installer_x\\d{2}\\.exe");

        public async Task<Asset?> GetLatestAsset()
        {
            var is64Bit = Environment.Is64BitOperatingSystem;
            var regex = is64Bit ? X64_BIT_REGEX : X86_BIT_REGEX;
            var assets = await getLatestAssets();
            return assets.Find(asset => regex.IsMatch(asset.Name));
        }

        private async Task<List<Asset>> getLatestAssets()
        {
            var release = await getLatestRelease();
            return release.Assets.FindAll(asset => INSTALLER_REGEX.IsMatch(asset.Name));
        }

        private async Task<Release> getLatestRelease()
        {
            var releases = await getReleases();
            return releases.First();
        }

        private async Task<List<Release>> getReleases()
        {
            HttpClient client = getHttpClient();
            var resultStream = await client.GetStreamAsync(RELEASES_URL);
            var releases = await JsonSerializer.DeserializeAsync<List<Release>>(resultStream);
            return releases ?? new List<Release>();
        }

        private HttpClient getHttpClient()
        {
            HttpClient client = new();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "ungoogled-chromium-updater");
            return client;
        }
    }
}
