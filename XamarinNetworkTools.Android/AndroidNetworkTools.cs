using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using InternalDroidNetworkTools;
using InternalDroidNetworkTools._Ping;
using InternalDroidNetworkTools.Subnet;
using Java.Net;

namespace XamarinNetworkTools
{
	public sealed class AndroidNetworkTools : INetworkTools
	{
		public static void Init() { }

		public static string LocalIPAddress => IPTools.getLocalIPv4Address().HostName;

		public static async Task<NetworkDevice> Ping(string ipAddress)
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
			});
		}

		public static IEnumerable<string> GetAllHostsForLocalIP()
		{
			return SubnetDevices.fromIPAddress(LocalIPAddress)
				.Addresses;
		}

		public static IObservable<NetworkDevice> FindDevicesOnNetwork()
		{
			return Observable.Create(async (IObserver<NetworkDevice> subscriber) =>
			{
				try
				{
					IEnumerable<string> localHosts = await Task.Run(() => AndroidNetworkTools.GetAllHostsForLocalIP());

					var pingTasks = localHosts.Select(x => Task.Run(async () =>
					{
						try
						{
							return await NetworkTools.Instance.Ping(x);
						}
						catch (Exception ex)
						{
							return (NetworkDevice)null;
						}
					})).ToList();

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

		IObservable<NetworkDevice> INetworkTools.FindDevicesOnNetwork() => AndroidNetworkTools.FindDevicesOnNetwork();

		string INetworkTools.LocalIPAddress => AndroidNetworkTools.LocalIPAddress;

		IEnumerable<string> INetworkTools.GetAllHostsForLocalIP() => AndroidNetworkTools.GetAllHostsForLocalIP();

		Task<NetworkDevice> INetworkTools.Ping(string ipAddress) => AndroidNetworkTools.Ping(ipAddress);

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