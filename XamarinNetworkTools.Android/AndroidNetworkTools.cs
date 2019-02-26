using System;
using System.Collections.Generic;

using System.Reactive.Linq;

using AndroidNetworkTools;
using static AndroidNetworkTools.SubnetDevices;

namespace XamarinNetworkTools
{
	public sealed class AndroidNetworkTools : INetworkTools
	{
		public static void Init() { }

		public static IObservable<NetworkDevice> FindDevicesOnNetwork()
		{
			return Observable.Create((IObserver<NetworkDevice> subscriber) =>
			{
				List<string> recievedIps = new List<string>();
				var scanner = SubnetDevices.fromLocalAddress()
					.findDevices(new SubnetCallbacks((device) =>
					{
						if (!recievedIps.Contains(device.IP))
						{
							recievedIps.Add(device.IP)
							subscriber.OnNext(new NetworkDevice(device.IP, string.IsNullOrEmpty(device.HostName) ? device.IP : device.HostName, device.MacAddress));
						}
					},
					() => { subscriber.OnCompleted(); }));

				return () =>
				{
					/* dispose me */
					scanner.Dispose();
				};
			});
		}

		IObservable<NetworkDevice> INetworkTools.FindDevicesOnNetwork() => AndroidNetworkTools.FindDevicesOnNetwork();

		class SubnetCallbacks : OnSubnetDeviceFound
		{
			Action<global::AndroidNetworkTools.Subnet.Device> onDeviceFound;
			Action onFinished;

			public SubnetCallbacks(Action<global::AndroidNetworkTools.Subnet.Device> onDeviceFound, Action onFinished)
			{
				this.onDeviceFound = onDeviceFound;
				this.onFinished = onFinished;
			}

			void OnSubnetDeviceFound.onDeviceFound(global::AndroidNetworkTools.Subnet.Device device)
			{
				this.onDeviceFound(device);
			}

			void OnSubnetDeviceFound.onFinished(List<global::AndroidNetworkTools.Subnet.Device> devicesFound) => this.onFinished();
		}
	}
}