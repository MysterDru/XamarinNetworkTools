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
				List<string> recievedIps = new List<string>();
				var lanScanner = new MMLANScanner(new LanScannerCallbacks(
					(MMDevice device) =>
					{
						if (!recievedIps.Contains(device.IpAddress))
						{
							recievedIps.Add(device.IpAddress);
							subscriber.OnNext(new NetworkDevice(device.IpAddress, string.IsNullOrWhiteSpace(device.Hostname) ? device.IpAddress : device.Hostname, device.MacAddress));
						}
					},
					(status) =>
					{
						if (status == MMLanScannerStatus.Finished)
							subscriber.OnCompleted();
						else
							subscriber.OnError(new OperationCanceledException("The scan was cancelled"));
					},
					() => { subscriber.OnError(new ScanFailedException()); }
				));

				lanScanner.Start();

				return () =>
				{
					/* dispose me */
					lanScanner.Stop();
					lanScanner.Dispose();
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
