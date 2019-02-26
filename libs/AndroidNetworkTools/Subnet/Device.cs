using System;
using Java.Net;

namespace AndroidNetworkTools.Subnet
{
	public class Device
	{
		public string IP;
		public string HostName;
		public string MacAddress;
		public float time = 0;

		public Device(InetAddress ip)
		{
			this.IP = ip.HostAddress;
			this.HostName = ip.CanonicalHostName;
		}

		public override string ToString()
		{
			return "Device{" +
						"ip='" + IP + '\'' +
						", hostname='" + HostName + '\'' +
						", mac='" + MacAddress + '\'' +
						", time=" + time +
						'}';
		}
	}
}
