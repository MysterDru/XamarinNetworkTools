using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Java.Net;

namespace InternalDroidNetworkTools
{
	internal static class IPTools
	{
		public static bool isIPv4Address(string address)
		{
			IPAddress ipa;
			return IPAddress.TryParse(address, out ipa);
		}

		public static bool isIPv6StdAddress(String address)
		{
			IPAddress ipa;
			return IPAddress.TryParse(address, out ipa);
		}

		public static bool isIPv6HexCompressedAddress(string address)
		{
			IPAddress ipa;
			return IPAddress.TryParse(address, out ipa);
		}

		public static bool isIPv6Address(string address)
		{
			IPAddress ipa;
			return IPAddress.TryParse(address, out ipa);
		}

		/**
		 * @return The first local IPv4 address, or null
		 */
		public static InetAddress getLocalIPv4Address()
		{
			List<InetAddress> localAddresses = getLocalIPv4Addresses();
			return localAddresses.Count > 0 ? localAddresses[0] : null;
		}

		/**
		 * @return The list of all IPv4 addresses found
		 */
		public static List<InetAddress> getLocalIPv4Addresses()
		{

			List<InetAddress> foundAddresses = new List<InetAddress>();

			Java.Util.IEnumeration ifaces;
			try
			{
				ifaces = NetworkInterface.NetworkInterfaces;

				while (ifaces.HasMoreElements)
				{
					NetworkInterface iface = ifaces.NextElement() as NetworkInterface;
					Java.Util.IEnumeration addresses = iface.InetAddresses;

					while (addresses.HasMoreElements)
					{
						InetAddress addr = addresses.NextElement() as InetAddress;
						if (addr is Inet4Address && !addr.IsLoopbackAddress)
						{
							foundAddresses.Add(addr);
						}
					}
				}
			}
			catch (SocketException e)
			{
				Console.WriteLine(e.Message);
			}
			return foundAddresses;
		}


		/**
		 * Check if the provided ip address refers to the localhost
		 *
		 * https://stackoverflow.com/a/2406819/315998
		 *
		 * @param addr - address to check
		 * @return - true if ip address is self
		 */
		public static bool isIpAddressLocalhost(InetAddress addr)
		{
			if (addr == null) return false;

			// Check if the address is a valid special local or loop back
			if (addr.IsAnyLocalAddress || addr.IsLoopbackAddress)
				return true;

			// Check if the address is defined on any interface
			try
			{
				return NetworkInterface.GetByInetAddress(addr) != null;
			}
			catch (SocketException e)
			{
				return false;
			}
		}

		/**
		 * Check if the provided ip address refers to the localhost
		 *
		 * https://stackoverflow.com/a/2406819/315998
		 *
		 * @param addr - address to check
		 * @return - true if ip address is self
		 */
		public static bool isIpAddressLocalNetwork(InetAddress addr)
		{
			return addr != null && addr.IsSiteLocalAddress;
		}
	}
}
