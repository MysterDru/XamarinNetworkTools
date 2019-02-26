using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AndroidNetworkTools;
using AndroidNetworkTools._Ping;
using AndroidNetworkTools.Subnet;
using Java.Lang;
using Java.Net;
using Java.Util.Concurrent;

namespace AndroidNetworkTools
{
	public class SubnetDevices : Java.Lang.Object
	{
		private int noThreads = 100;

		private List<string> addresses;
		private ConcurrentStack<Device> devicesFound;
		private OnSubnetDeviceFound listener;
		private int timeOutMillis = 2500;
		private bool cancelled = false;
		private Dictionary<string, string> ipMacHashMap = null;

		/**
	* Find devices on the subnet working from the local device ip address
	*
	* @return - this for chaining
	*/
		public static SubnetDevices fromLocalAddress()
		{
			InetAddress ipv4 = IPTools.getLocalIPv4Address();

			if (ipv4 == null)
			{
				throw new IllegalAccessError("Could not access local ip address");
			}

			return fromIPAddress(ipv4.HostAddress);
		}

		/**
		 * @param inetAddress - an ip address in the subnet
		 *
		 * @return - this for chaining
		 */
		public static SubnetDevices fromIPAddress(InetAddress inetAddress)
		{
			return fromIPAddress(inetAddress.HostAddress);
		}

		/**
		 * @param ipAddress - the ipAddress string of any device in the subnet i.e. "192.168.0.1"
		 *                  the final part will be ignored
		 *
		 * @return - this for chaining
		 */
		public static SubnetDevices fromIPAddress(string ipAddress)
		{

			if (!IPTools.isIPv4Address(ipAddress))
			{
				throw new IllegalArgumentException("Invalid IP Address");
			}

			SubnetDevices subnetDevice = new SubnetDevices();

			subnetDevice.addresses = new List<string>();

			// Get addresses from ARP Info first as they are likely to be reachable
			subnetDevice.addresses.AddRange(ARPInfo.getAllIPAddressesInARPCache());

			// Add all missing addresses in subnet
			string segment = ipAddress.Substring(0, ipAddress.LastIndexOf('.') + 1);
			for (int j = 0; j < 255; j++)
			{
				if (!subnetDevice.addresses.Contains(segment + j))
				{
					subnetDevice.addresses.Add(segment + j);
				}
			}

			return subnetDevice;

		}

		/**
     * Starts the scan to find other devices on the subnet
     *
     * @param listener - to pass on the results
     * @return this object so we can call cancel on it if needed
     */
		public SubnetDevices findDevices(OnSubnetDeviceFound listener)
		{

			this.listener = listener;

			cancelled = false;
			devicesFound = new ConcurrentStack<Device>();

			var self = this;
			new Thread(new Runnable(() =>
			{

				// Load mac addresses into cache var (to avoid hammering the /proc/net/arp file when
				// lots of devices are found on the network.
				ipMacHashMap = ARPInfo.getAllIPAndMACAddressesInARPCache();

				IExecutorService executor = Executors.NewFixedThreadPool(noThreads);

				foreach (string add in addresses)
				{
					IRunnable worker = new SubnetDeviceFinderRunnable(add, self);
					executor.Execute(worker);
				}

				// This will make the executor accept no new threads
				// and finish all existing threads in the queue
				executor.Shutdown();
				// Wait until all threads are finish
				try
				{
					executor.AwaitTermination(1, TimeUnit.Hours);
				}
				catch (InterruptedException e)
				{
					e.PrintStackTrace();
				}

				// Loop over devices found and add in the MAC addresses if missing.
				// We do this after scanning for all devices as /proc/net/arp may add info
				// because of the scan.
				ipMacHashMap = ARPInfo.getAllIPAndMACAddressesInARPCache();
				foreach (Device device in devicesFound)
				{
					if (device.MacAddress == null && ipMacHashMap.ContainsKey(device.IP))
					{
						device.MacAddress = ipMacHashMap[device.IP];
					}
				}

				listener.onFinished(devicesFound.ToList());
			})).Start();

			return this;
		}

		private void subnetDeviceFound(Device device)
		{
			devicesFound.Push(device);
			listener.onDeviceFound(device);
		}

		public class SubnetDeviceFinderRunnable : Java.Lang.Object, IRunnable
		{
			private SubnetDevices parent;
			private string address;

			public SubnetDeviceFinderRunnable(string address, SubnetDevices parent)
			{
				this.address = address;
				this.parent = parent;
			}

			public void Run()
			{


				if (parent.cancelled) return;

				try
				{
					InetAddress ia = InetAddress.GetByName(address);
					PingResult pingResult = Ping.onAddress(ia).setTimeOutMillis(parent.timeOutMillis).doPing();
					if (pingResult.IsReachable)
					{
						Device device = new Device(ia);

						// Add the device MAC address if it is in the cache
						if (parent.ipMacHashMap.ContainsKey(ia.HostAddress))
						{
							device.MacAddress = parent.ipMacHashMap[ia.HostAddress];
						}

						device.time = pingResult.TimeTaken;

						parent.subnetDeviceFound(device);
					}
				}
				catch (UnknownHostException e)
				{
					e.PrintStackTrace();
				}
			}
		}

		public interface OnSubnetDeviceFound
		{
			void onDeviceFound(Device device);

			void onFinished(List<Device> devicesFound);
		}
	}

}

