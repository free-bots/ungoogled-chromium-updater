// See https://aka.ms/new-console-template for more information
using ungoogled_chromium_updater.network;
using ungoogled_chromium_updater.updater;

namespace ungoogled_chromium_updater
{
    public class Program
    {
        private static readonly TimeSpan INTERVAL = TimeSpan.FromHours(6);

        private static readonly UpdaterService updaterService = new();
        private static readonly NetworkAvailabilityService networkAvailabilityService = new();

        public static async Task Main(string[] args)
        {
            await CheckForUpdate();

            while (await WaitForEvents())
            {
                await CheckForUpdate();
            }
        }

        private static async Task CheckForUpdate()
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

        private static Task<bool> WaitForEvents()
        {
            var tasks = new Task<bool>[]
            {
                                WaitForTimer(),
                                networkAvailabilityService.WaitForAvailableNetwork()
            };
            return Task.WhenAny(tasks).Result;
        }

        private static Task<bool> WaitForTimer()
        {
            return new PeriodicTimer(INTERVAL)
                .WaitForNextTickAsync()
                .AsTask();
        }
    }
}
