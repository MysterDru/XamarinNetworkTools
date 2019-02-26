using System;
using System.Collections.Generic;

namespace XamarinNetworkTools
{
	public interface INetworkTools
	{
		IObservable<NetworkDevice> FindDevicesOnNetwork();
	}
}