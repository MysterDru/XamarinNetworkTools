using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XamarinNetworkTools
{
	public interface INetworkTools
	{
		IObservable<NetworkDevice> FindDevicesOnNetwork();

		string LocalIPAddress { get; }

		IEnumerable<string> GetAllHostsForLocalIP();

		Task<NetworkDevice> Ping(string ipAddress);

	}
}