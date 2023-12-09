using System.Net.NetworkInformation;

namespace ungoogled_chromium_updater.network
{
    public class NetworkAvailabilityService
    {
        public async Task<bool> WaitForAvailableNetwork()
        {
            int lastInterfacesOnline = GetOnlineNetworkInterfaces();
            TaskCompletionSource<bool> networkAvailableSource = new();
            
            NetworkAvailabilityChangedEventHandler networkAvailabilityChangedEventHandler = OnAvalabilityChanged(lastInterfacesOnline, networkAvailableSource);
            NetworkAddressChangedEventHandler networkAddressChangedEventHandler = OnNetworkAddressChanged(lastInterfacesOnline, networkAvailableSource);

            NetworkChange.NetworkAvailabilityChanged += networkAvailabilityChangedEventHandler;
            NetworkChange.NetworkAddressChanged += networkAddressChangedEventHandler;

            try
            {
                return await networkAvailableSource.Task;
            } catch(Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return false;
            } finally
            {
                NetworkChange.NetworkAvailabilityChanged -= networkAvailabilityChangedEventHandler;
                NetworkChange.NetworkAddressChanged -= networkAddressChangedEventHandler;
            }
        }

        private NetworkAvailabilityChangedEventHandler OnAvalabilityChanged(int interfacesOnline, TaskCompletionSource<bool> networkAvailableSource)
        {
            return (object? sender, NetworkAvailabilityEventArgs e) =>
        {
            if (e.IsAvailable)
            {
                networkAvailableSource.TrySetResult(true);
            }
        };
        }

        private NetworkAddressChangedEventHandler OnNetworkAddressChanged(int interfacesOnline, TaskCompletionSource<bool> networkAvailableSource)
        {
            return (object? sender, EventArgs e) =>        {
                int currentOnline = GetOnlineNetworkInterfaces();
                if (interfacesOnline != currentOnline && currentOnline > 0)
                {
                    networkAvailableSource.TrySetResult(true);
                }
            };
        }

        private static int GetOnlineNetworkInterfaces()
        {
            return NetworkInterface.GetAllNetworkInterfaces()
                .Where(networkInterface => networkInterface.OperationalStatus == OperationalStatus.Up)
                .Count();
        }
    }
}
