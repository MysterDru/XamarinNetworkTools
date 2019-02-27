using System;
using System.Threading;
using System.Threading.Tasks;
using InternalDroidNetworkTools._Ping;
using Java.Lang;
using Java.Net;

namespace InternalDroidNetworkTools
{
	internal class Ping
	{
		// This class is not to be instantiated
		private Ping()
		{
		}

		public interface PingListener
		{
			void onResult(PingResult pingResult);
			void onFinished(PingStats pingStats);
			void onError(System.Exception e);
		}

		private string addressString = null;
		private InetAddress address;
		private PingOptions pingOptions = new PingOptions();
		private int delayBetweenScansMillis = 0;
		private int times = 1;
		private bool cancelled = false;

		/**
		 * Set the address to ping
		 *
		 * Note that a lookup is not performed here so that we do not accidentally perform a network
		 * request on the UI thread.
		 *
		 * @param address - Address to be pinged
		 * @return this object to allow chaining
		 */
		public static Ping onAddress(string address)
		{
			Ping ping = new Ping();
			ping.setAddressString(address);
			return ping;
		}

		/**
		 * Set the address to ping
		 *
		 * @param ia - Address to be pinged
		 * @return this object to allow chaining
		 */
		public static Ping onAddress(InetAddress ia)
		{
			Ping ping = new Ping();
			ping.setAddress(ia);
			return ping;
		}

		/**
		 * Set the timeout
		 *
		 * @param timeOutMillis - the timeout for each ping in milliseconds
		 * @return this object to allow chaining
		 */
		public Ping setTimeOutMillis(int timeOutMillis)
		{
			if (timeOutMillis < 0) throw new ArgumentException("Times cannot be less than 0");
			pingOptions.TimeoutMillis = timeOutMillis;
			return this;
		}

		/**
		 * Set the delay between each ping
		 *
		 * @param delayBetweenScansMillis - the timeout for each ping in milliseconds
		 * @return this object to allow chaining
		 */
		public Ping setDelayMillis(int delayBetweenScansMillis)
		{
			if (delayBetweenScansMillis < 0)
				throw new ArgumentException("Delay cannot be less than 0");
			this.delayBetweenScansMillis = delayBetweenScansMillis;
			return this;
		}

		/**
		 * Set the time to live
		 *
		 * @param timeToLive - the TTL for each ping
		 * @return this object to allow chaining
		 */
		public Ping setTimeToLive(int timeToLive)
		{
			if (timeToLive < 1) throw new ArgumentException("TTL cannot be less than 1");
			pingOptions.TimeToLive = timeToLive;
			return this;
		}

		/**
		 * Set number of times to ping the address
		 *
		 * @param noTimes - number of times, 0 = continuous
		 * @return this object to allow chaining
		 */
		public Ping setTimes(int noTimes)
		{
			if (noTimes < 0) throw new ArgumentException("Times cannot be less than 0");
			this.times = noTimes;
			return this;
		}

		private void setAddress(InetAddress address)
		{
			this.address = address;
		}

		/**
		 * Set the address string which will be resolved to an address by resolveAddressString()
		 *
		 * @param addressString - String of the address to be pinged
		 */
		private void setAddressString(string addressString)
		{
			this.addressString = addressString;
		}

		/// <summary>
		/// Parses the addressString to an address
		/// </summary>
		/// <exception cref="UnknownHostException" />
		private void resolveAddressString()
		{
			if (address == null && addressString != null)
				address = InetAddress.GetByName(addressString);
		}


		/**
		 * Cancel a running ping
		 */
		public void cancel()
		{
			this.cancelled = true;
		}

		/**
		 * Perform a synchronous ping and return a result, will ignore number of times.
		 *
		 * Note that this should be performed on a background thread as it will perform a network
		 * request
		 *
		 * @return - ping result
		 * @throws UnknownHostException - if the host cannot be resolved
		 */
		public PingResult doPing()
		{
			cancelled = false;
			resolveAddressString();
			return PingTools.doPing(address, pingOptions);
		}
	}
}