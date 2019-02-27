using System;
using Java.Lang;
using Java.Net;

namespace InternalDroidNetworkTools._Ping
{
	internal class PingTools
	{

		// This class is not to be instantiated
		private PingTools()
		{
		}


		/**
		 * Perform a ping using the native ping tool and fall back to using java echo request
		 * on failure.
		 *
		 * @param ia            - address to ping
		 * @param pingOptions   - ping command options
		 * @return - the ping results
		 */
		public static PingResult doPing(InetAddress ia, PingOptions pingOptions)
		{
			// the origianl android java source has a native ping method, but we'll skip that

			// Fallback to java based ping
			return PingTools.doJavaPing(ia, pingOptions);
		}

		/**
		 * Tries to reach this {@code InetAddress}. This method first tries to use
		 * ICMP <i>(ICMP ECHO REQUEST)</i>, falling back to a TCP connection
		 * on port 7 (Echo) of the remote host.
		 *
		 * @param ia            - address to ping
		 * @param pingOptions   - ping command options
		 * @return - the ping results
		 */
		private static PingResult doJavaPing(InetAddress ia, PingOptions pingOptions)
		{
			PingResult pingResult = new PingResult(ia);

			if (ia == null)
			{
				pingResult.IsReachable = false;
				return pingResult;
			}

			try
			{
				long startTime = Java.Lang.JavaSystem.NanoTime();
				bool reached = ia.IsReachable(null, pingOptions.TimeToLive, pingOptions.TimeoutMillis);
				pingResult.TimeTaken = (JavaSystem.NanoTime() - startTime) / 1e6f;
				pingResult.IsReachable = reached;
				if (!reached) pingResult.Error = "Timed Out";
			}
			catch (System.Exception e)
			{
				pingResult.IsReachable = false;
				pingResult.Error = "Exception: " + e.Message;
			}
			return pingResult;
		}
	}

}
