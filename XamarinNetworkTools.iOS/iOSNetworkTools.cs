using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MMLanScanBinding;

namespace XamarinNetworkTools
{
	public class iOSNetworkTools : INetworkTools
	{
		public static void Init() { }

		public static string LocalIPAddress => LANProperties.LocalIPAddress?.IpAddress;

		public static IEnumerable<string> GetAllHostsForLocalIP()
		{
			var localDevice = LANProperties.LocalIPAddress;

			var ipsToPing = LANProperties.GetAllHostsForIP(localDevice.IpAddress, localDevice.SubnetMask);

			List<string> toReturn = new List<string>();
			foreach (var ip in ipsToPing)
				toReturn.Add(ip.ToString());

			return toReturn;
		}

		public static IObservable<NetworkDevice> FindDevicesOnNetwork()
		{
			return Observable.Create(async (IObserver<NetworkDevice> subscriber) =>
			{
				try
				{
					IEnumerable<string> localHosts = await Task.Run(() => iOSNetworkTools.GetAllHostsForLocalIP());

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
				catch(Exception ex)
				{
					subscriber.OnError(ex);
				}

				//Console.WriteLine($"[XamarinNetworkTools] - Scan Started");
				//var lanScanner = new MMLANScanner(new LanScannerCallbacks(
				//	(MMDevice device) =>
				//	{
				//		Console.WriteLine("[XamarinNetworkTools] - Found new device: Device{" +
				//			"ip='" + device.IpAddress + '\'' +
				//			", hostname='" + device.Hostname + '\'' +
				//			", mac='" + device.MacAddress + '\'' +
				//			", isLocal=" + device.IsLocalDevice +
				//			'}');

				//		subscriber.OnNext(new NetworkDevice(device.IpAddress, string.IsNullOrWhiteSpace(device.Hostname) ? device.IpAddress : device.Hostname, device.MacAddress));
				//	},
				//	(status) =>
				//	{
				//		Console.WriteLine($"[XamarinNetworkTools] - Scan finished with status: {status}");
				//		if (status == MMLanScannerStatus.Finished)
				//			subscriber.OnCompleted();
				//		else
				//			subscriber.OnError(new OperationCanceledException("The scan was cancelled"));
				//	},
				//	() =>
				//	{
				//		Console.WriteLine($"[XamarinNetworkTools] - Scan failed with error: 'Unknown'");
				//		subscriber.OnError(new ScanFailedException());
				//	}
				//));

				//lanScanner.Start();

				//return () =>
				//{
				//	Console.WriteLine($"[XamarinNetworkTools] - Disposing {nameof(FindDevicesOnNetwork)} observable");

				//	/* dispose me */
				//	lanScanner?.Stop();
				//	lanScanner?.Dispose();
				//};
			});
		}

		public static async Task<NetworkDevice> Ping(string ipAddress)
		{
			TaskCompletionSource<NetworkDevice> tcs = new TaskCompletionSource<NetworkDevice>();
			await Task.Run(() =>
			{
				var ping = new PingOperation(ipAddress, (Foundation.NSError arg1, Foundation.NSString arg2) =>
				{
					if (arg1 != null)
						tcs.TrySetException(new Exception(arg1.Description));
					else
					{
						tcs.TrySetResult(new NetworkDevice(ipAddress, ipAddress, null));
					}
				});
				ping.Start();
			});

			return await tcs.Task;
		}

		#region INetworkTools implementation

		IObservable<NetworkDevice> INetworkTools.FindDevicesOnNetwork() => iOSNetworkTools.FindDevicesOnNetwork();

		string INetworkTools.LocalIPAddress => iOSNetworkTools.LocalIPAddress;

		IEnumerable<string> INetworkTools.GetAllHostsForLocalIP() => iOSNetworkTools.GetAllHostsForLocalIP();

		Task<NetworkDevice> INetworkTools.Ping(string ipAddress) => iOSNetworkTools.Ping(ipAddress);

		#endregion

		private class LanScannerCallbacks : MMLANScannerDelegate
		{
			Action<MMDevice> onDeviceFound;
			Action<MMLanScannerStatus> onFinished;
			Action onFailedToScan;

			public LanScannerCallbacks(Action<MMDevice> onDeviceFound, Action<MMLanScannerStatus> onFinished, Action onFailedToScan)
			{
				this.onDeviceFound = onDeviceFound;
				this.onFinished = onFinished;
				this.onFailedToScan = onFailedToScan;
			}

			public override void LanScanProgressPinged(float pingedHosts, nint overallHosts)
			{
			}

			public override void LanScanDidFailedToScan()
			{
				this.onFailedToScan();
			}

			public override void LanScanDidFindNewDevice(MMLanScanBinding.MMDevice device)
			{
				this.onDeviceFound(device);
			}

			public override void LanScanDidFinishScanningWithStatus(MMLanScannerStatus status)
			{
				this.onFinished(status);
			}
		}
	}
}
