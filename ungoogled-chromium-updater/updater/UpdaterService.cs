using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using ungoogled_chromium_updater.chromium;
using ungoogled_chromium_updater.github;
using ungoogled_chromium_updater.github.models;

namespace ungoogled_chromium_updater.updater
{
    public class UpdaterService
    {
        private static readonly GithubService githubService = new();
        private static readonly ChromiumService chromiumService = new();

        public async Task Update()
        {
            var currentVersion = chromiumService.GetChromiumVersion();
            if (currentVersion == null)
            {
                Console.WriteLine("Chromium not installed ignoring");
                return;
            }

            var asset = await githubService.GetLatestAsset();
            if (asset == null || asset.Name.Contains(currentVersion))
            {
                Console.WriteLine("No update found");
                return;
            }

            await downloadAndInstall(asset);

            var newVersion = chromiumService.GetChromiumVersion();
            if (newVersion == null)
            {
                Console.WriteLine("Could not detect new version");
                return;
            }

            showUpdateNotification(newVersion);
        }

        private async Task downloadAndInstall(Asset asset)
        {
            var response = await downloadInstaller(asset.DownloadUrl);
            var path = getFilePath();

            File.WriteAllBytes(path, response);

            var process = Process.Start(path);
            process.WaitForExit();
            process.Close();

            File.Delete(path);
        }

        private Task<byte[]> downloadInstaller(string url)
        {
            HttpClient client = new HttpClient();
            return client.GetByteArrayAsync(url);
        }

        private string getFilePath()
        {
            var guid = Guid.NewGuid().ToString();
            return Path.Combine(Path.GetTempPath(), $"{guid}-ungoogled-chromium.exe");
        }

        private void showUpdateNotification(string version)
        {
            new ToastContentBuilder()
            .AddText($"ungoogled-chromium updated to version: {version}")
            .Show();
        }
    }
}
