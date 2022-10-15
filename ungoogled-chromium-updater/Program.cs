// See https://aka.ms/new-console-template for more information

using ungoogled_chromium_updater.updater;

namespace ungoogled_chromium_updater
{
    public class Program
    {
        private static readonly TimeSpan INTERVAL = TimeSpan.FromHours(6);

        private static readonly UpdaterService updaterService = new();

        public static async Task Main(string[] args)
        {
            await checkForUpdate();

            var timer = new PeriodicTimer(INTERVAL);
            while (await timer.WaitForNextTickAsync())
            {
                await checkForUpdate();
            }
        }

        private static async Task checkForUpdate()
        {
            try
            {
                await updaterService.Update();
            }
            catch (Exception exeption)
            {
                Console.Error.WriteLine(exeption);
            }
        }
    }
}
