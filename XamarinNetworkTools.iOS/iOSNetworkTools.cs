using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using MMLanScanBinding;

namespace XamarinNetworkTools
{
	public class iOSNetworkTools : INetworkTools
	{
		public static void Init() { }

		public static IObservable<NetworkDevice> FindDevicesOnNetwork()
		{
			return Observable.Create((IObserver<NetworkDevice> subscriber) =>
			{
				Console.WriteLine($"[XamarinNetworkTools] - Scan Started");
				var lanScanner = new MMLANScanner(new LanScannerCallbacks(
					(MMDevice device) =>
					{
						Console.WriteLine("[XamarinNetworkTools] - Found new device: Device{" +
							"ip='" + device.IpAddress + '\'' +
							", hostname='" + device.Hostname + '\'' +
							", mac='" + device.MacAddress + '\'' +
							", isLocal=" + device.IsLocalDevice +
							'}');

						subscriber.OnNext(new NetworkDevice(device.IpAddress, string.IsNullOrWhiteSpace(device.Hostname) ? device.IpAddress : device.Hostname, device.MacAddress));
					},
					(status) =>
					{
						Console.WriteLine($"[XamarinNetworkTools] - Scan finished with status: {status}");
						if (status == MMLanScannerStatus.Finished)
							subscriber.OnCompleted();
						else
							subscriber.OnError(new OperationCanceledException("The scan was cancelled"));
					},
					() => {
						Console.WriteLine($"[XamarinNetworkTools] - Scan failed with error: 'Unknown'");
						subscriber.OnError(new ScanFailedException()); 
					}
				));

				lanScanner.Start();

				return () =>
				{
					Console.WriteLine($"[XamarinNetworkTools] - Disposing {nameof(FindDevicesOnNetwork)} observable");

					/* dispose me */
					lanScanner?.Stop();
					lanScanner?.Dispose();
				};
			});
		}

		IObservable<NetworkDevice> INetworkTools.FindDevicesOnNetwork() => iOSNetworkTools.FindDevicesOnNetwork();

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
