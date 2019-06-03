using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using InternalDroidNetworkTools;
using InternalDroidNetworkTools._Ping;
using InternalDroidNetworkTools.Subnet;
using Java.Net;

namespace XamarinNetworkTools
{
    [Android.Runtime.Preserve(AllMembers = true)]
    public sealed class AndroidNetworkTools : INetworkTools
    {
        public static void Init()
        {
            XamarinNetworkTools.NetworkTools.Init<AndroidNetworkTools>();
        }

        public static bool IsSupported => true;

        public static string LocalIPAddress => IPTools.getLocalIPv4Address().HostName;

        public static async Task<NetworkDevice> Ping(string ipAddress, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                InetAddress ia = InetAddress.GetByName(ipAddress);
                PingResult pingResult = InternalDroidNetworkTools.Ping.onAddress(ia).setTimeOutMillis(2500).doPing();
                if (pingResult.IsReachable)
                {
                    Device device = new Device(ia);

                    return new NetworkDevice(device.IP, device.HostName, device.MacAddress);
                }
                else
                {
                    throw new Exception(pingResult.Error);
                }
            }, cancellationToken);
        }

        public static IEnumerable<string> GetAllHostsForLocalIP()
        {
            return SubnetDevices.fromIPAddress(LocalIPAddress)
                .Addresses;
        }

        public static IObservable<NetworkDevice> FindDevicesOnNetwork()
        {
            return Observable.Create(async (IObserver<NetworkDevice> subscriber, CancellationToken cancellationToken) =>
            {
                try
                {
                    IEnumerable<string> localHosts = await Task.Run(() => AndroidNetworkTools.GetAllHostsForLocalIP(), cancellationToken);

                    var pingTasks = localHosts.Select(x => Task.Run(async () =>
                    {
                        try
                        {
                            var device = await NetworkTools.Instance.Ping(x, cancellationToken);

#if DEBUG
                            Console.WriteLine($"Ping completed: {device.IP } | {device.HostName } | { device.MacAddress}");
#endif

                            return device;
                        }
                        catch
                        {
                            return (NetworkDevice)null;
                        }
                    }, cancellationToken)).ToList();

                    while (pingTasks.Count > 0)
                    {
                        var task = await Task.WhenAny(pingTasks);
                        pingTasks.Remove(task);

                        if (task.Result != null)
                            subscriber.OnNext(task.Result);
                    }

                    subscriber.OnCompleted();
                }
                catch (Exception ex)
                {
                    subscriber.OnError(ex);
                }
            });
        }

        #region INetworkTools Implementation

        bool INetworkTools.IsSupported => AndroidNetworkTools.IsSupported;

        IObservable<NetworkDevice> INetworkTools.FindDevicesOnNetwork() => AndroidNetworkTools.FindDevicesOnNetwork();

        string INetworkTools.LocalIPAddress => AndroidNetworkTools.LocalIPAddress;

        IEnumerable<string> INetworkTools.GetAllHostsForLocalIP() => AndroidNetworkTools.GetAllHostsForLocalIP();

        Task<NetworkDevice> INetworkTools.Ping(string ipAddress, CancellationToken cancellationToken) => AndroidNetworkTools.Ping(ipAddress, cancellationToken);

        #endregion

        class SubnetCallbacks : SubnetDevices.OnSubnetDeviceFound
        {
            Action<Device> onDeviceFound;
            Action onFinished;

            public SubnetCallbacks(Action<Device> onDeviceFound, Action onFinished)
            {
                this.onDeviceFound = onDeviceFound;
                this.onFinished = onFinished;
            }

            void SubnetDevices.OnSubnetDeviceFound.onDeviceFound(Device device)
            {
                this.onDeviceFound(device);
            }

            void SubnetDevices.OnSubnetDeviceFound.onFinished(List<Device> devicesFound) => this.onFinished();
        }
    }
}