using System;
using Java.Net;

namespace AndroidNetworkTools._Ping
{
	public class PingResult
	{
		public InetAddress IA { get;}

		public bool IsReachable { get; set; }

		public string Error { get; set; }

		public float TimeTaken { get; set; }

		public string FullString { get; set; }

		public string Result { get; set; }

		public bool HasError => string.IsNullOrEmpty(Error) == false;

		public PingResult(InetAddress ia)
		{
			this.IA = ia;
		}

		public override string ToString()
		{
			return "PingResult{" +
				"ia=" + IA +
				", isReachable=" + IsReachable +
				", error='" + Error + '\'' +
				", timeTaken=" + TimeTaken +
				", fullString='" + FullString + '\'' +
				", result='" + Result + '\'' +
				'}';
		}
	}
}
