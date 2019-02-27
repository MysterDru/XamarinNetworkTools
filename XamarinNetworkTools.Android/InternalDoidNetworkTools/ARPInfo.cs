using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Java.IO;
using Java.Lang;

namespace InternalDroidNetworkTools
{
	/**
	 * Created by mat on 09/12/15.
	 *
	 * Looks at the file at /proc/net/arp to fromIPAddress ip/mac addresses from the cache
	 * We assume that the file has this structure:
	 *
	 * IP address       HW type     Flags       HW address            Mask     Device
	 * 192.168.18.11    0x1         0x2         00:04:20:06:55:1a     *        eth0
	 * 192.168.18.36    0x1         0x2         00:22:43:ab:2a:5b     *        eth0
	 */
	internal class ARPInfo
	{

		// This class is not to be instantiated
		private ARPInfo()
		{
		}


		/**
		 * Try to extract a hardware MAC address from a given IP address using the
		 * ARP cache (/proc/net/arp).
		 *
		 * @param ip - IP address to search for
		 * @return the MAC from the ARP cache or null in format "01:23:45:67:89:ab"
		 */
		public static string getMACFromIPAddress(string ip)
		{
			if (ip == null)
			{
				return null;
			}

			foreach (string line in getLinesInARPCache())
			{
				var js = new Java.Lang.String(line);
				string[] splitted = js.Split(" +");
				if (splitted.Length >= 4 && string.Equals(ip, splitted[0]))
				{
					string mac = splitted[3];
					if (Regex.IsMatch(mac, "..:..:..:..:..:..")) 
					{
						return mac;
					}
					else
					{
						return null;
					}
				}
			}
			return null;
		}


		/**
		 * Try to extract a IP address from the given MAC address using the
		 * ARP cache (/proc/net/arp).
		 *
		 * @param macAddress in format "01:23:45:67:89:ab" to search for
		 * @return the IP address found or null in format "192.168.0.1"
		 */
		public static string getIPAddressFromMAC(string macAddress)
		{
			if (macAddress == null)
			{
				return null;
			}

			if (!Regex.IsMatch(macAddress, "..:..:..:..:..:.."))
			{
				throw new IllegalArgumentException("Invalid MAC Address");
			}

			foreach(string line in getLinesInARPCache())
			{
				var js = new Java.Lang.String(line);
				string[] splitted = js.Split(" +");
				if (splitted.Length >= 4 && string.Equals(macAddress, splitted[3]))
				{
					return splitted[0];
				}
			}
			return null;
		}

		/**
		 * Returns all the ip addresses currently in the ARP cache (/proc/net/arp).
		 *
		 * @return list of IP addresses found
		 */
		public static List<string> getAllIPAddressesInARPCache()
		{
			return new List<string>(getAllIPAndMACAddressesInARPCache().Keys);
		}

		/**
		 * Returns all the MAC addresses currently in the ARP cache (/proc/net/arp).
		 *
		 * @return list of MAC addresses found
		 */
		public static List<string> getAllMACAddressesInARPCache()
		{
			return new List<string>(getAllIPAndMACAddressesInARPCache().Values);
		}


		/**
		 * Returns all the IP/MAC address pairs currently in the ARP cache (/proc/net/arp).
		 *
		 * @return list of IP/MAC address pairs found
		 */
		public static Dictionary<string,string> getAllIPAndMACAddressesInARPCache()
		{
			Dictionary<string, string> macList = new Dictionary<string, string>();
			foreach(string line in getLinesInARPCache())
			{
				string[] splitted = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				if (splitted.Length >= 4)
				{
					// Ignore values with invalid MAC addresses
					if (Regex.IsMatch(splitted[3].Trim(), "..:..:..:..:..:..")
							&& !string.Equals(splitted[3].Trim(), "00:00:00:00:00:00"))
					{
						macList.Add(splitted[0].Trim(), splitted[3].Trim());
					}
				}
			}
			return macList;
		}

		/**
		 * Method to read lines from the ARP Cache
		 *
		 * @return the lines of the ARP Cache.
		 */
		private static List<string> getLinesInARPCache()
		{
			List<string> lines = new List<string>();
			BufferedReader br = null;
			try
			{
				br = new BufferedReader(new FileReader("/proc/net/arp"));
				string line;
				while ((line = br.ReadLine()) != null)
				{
					lines.Add(line);
				}
			}
			catch (Java.Lang.Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
			finally
			{
				try
				{
					if (br != null)
					{
						br.Close();
					}
				}
				catch (IOException e)
				{
					System.Console.WriteLine(e.Message);
				}
			}
			return lines;
		}

	}
}
