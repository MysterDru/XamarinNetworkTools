using System;
namespace AndroidNetworkTools._Ping
{
	public class PingOptions
	{
		private int timeoutMillis;
		private int timeToLive;

		public int TimeoutMillis
		{
			get => this.timeoutMillis;
			set => this.timeoutMillis = Math.Max(value, 1000);
		}

		public int TimeToLive
		{
			get => this.timeToLive;
			set => this.timeToLive = Math.Max(value, 1);
		}

		public PingOptions()
		{
			timeToLive = 128;
			timeoutMillis = 1000;
		}
	}
}
