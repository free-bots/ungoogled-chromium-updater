using System.Text.RegularExpressions;

namespace ungoogled_chromium_updater.chromium
{
    public class ChromiumService
    {
        private static readonly Regex VERSION_REGEX = new("([0-9\\.]+)");
        private static readonly string CHROMIUM_PATH = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Chromium",
            "Application"
            );

        public string? GetChromiumVersion()
        {
            if (!Directory.Exists(CHROMIUM_PATH))
            {
                return null;
            }

            var directoryInfo = new DirectoryInfo(CHROMIUM_PATH);
            return directoryInfo.GetDirectories()
                .Where(info => VERSION_REGEX.IsMatch(info.Name))
                .OrderByDescending(info => info.CreationTime)
                .Select(info => VERSION_REGEX.Matches(info.Name).First().Value)
                .FirstOrDefault();
        }

    }
}
