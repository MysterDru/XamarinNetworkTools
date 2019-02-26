using System;
using System.Linq;
using System.Collections.Generic;

namespace XamarinNetworkTools
{
	public static class NetworkTools
	{
		public static INetworkTools Instance { get; private set; }

		static NetworkTools()
		{
			var platformAssembly = AppDomain.CurrentDomain.GetAssemblies()
				.Where(x => x.FullName.Contains("XamarinNetworkTools.iOS") || x.FullName.Contains("XamarinNetworkTools.Android"))
				.FirstOrDefault();

			var platformType = platformAssembly.GetTypes()
				.FirstOrDefault(x => typeof(INetworkTools).IsAssignableFrom(x));

			Instance = Activator.CreateInstance(platformType) as INetworkTools;
		}

		public static IObservable<NetworkDevice> FindDevicesOnNetwork()
			=> Instance.FindDevicesOnNetwork();
	}
}
