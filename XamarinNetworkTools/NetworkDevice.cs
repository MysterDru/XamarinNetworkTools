using System;
namespace XamarinNetworkTools
{
	public class NetworkDevice
	{
		public string IP { get; set; }

		public string HostName { get; set; }

		public string MacAddress { get; set; }

		public NetworkDevice(string ip, string hostname, string mac)
		{
			this.IP = ip;
			this.HostName = hostname;
			this.MacAddress = mac;
		}
	}
}