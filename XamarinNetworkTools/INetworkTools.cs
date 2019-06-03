using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace XamarinNetworkTools
{
	public interface INetworkTools
	{
        bool IsSupported { get; }

		IObservable<NetworkDevice> FindDevicesOnNetwork();

		string LocalIPAddress { get; }

		IEnumerable<string> GetAllHostsForLocalIP();

		Task<NetworkDevice> Ping(string ipAddress, CancellationToken cancellationToken = default(CancellationToken));
	}
}