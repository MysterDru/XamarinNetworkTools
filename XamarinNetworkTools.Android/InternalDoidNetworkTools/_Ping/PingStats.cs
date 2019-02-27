using System;
using Java.Net;

namespace InternalDroidNetworkTools._Ping
{
	internal class PingStats
	{
		private readonly InetAddress ia;
		private readonly long noPings;
		private readonly long packetsLost;
		private readonly float averageTimeTaken;
		private readonly float minTimeTaken;
		private readonly float maxTimeTaken;
		private readonly bool isReachable;

		public PingStats(InetAddress ia, long noPings, long packetsLost, float totalTimeTaken, float minTimeTaken, float maxTimeTaken)
		{
			this.ia = ia;
			this.noPings = noPings;
			this.packetsLost = packetsLost;
			this.averageTimeTaken = totalTimeTaken / noPings;
			this.minTimeTaken = minTimeTaken;
			this.maxTimeTaken = maxTimeTaken;
			this.isReachable = noPings - packetsLost > 0;
		}

		public InetAddress Address => ia;

		public long NoPings => noPings;

		public long PacketsLost => packetsLost; 

		public float AverageTimeTaken => averageTimeTaken;

		public float MinTimeTaken => minTimeTaken;

		public float MaxTimeTaken => maxTimeTaken;

		public bool IsReachable() => isReachable;

		public long  AverageTimeTakenMillis => (long)(averageTimeTaken * 1000);

		public long MinTimeTakenMillis => (long)(minTimeTaken * 1000);

		public long MaxTimeTakenMillis => (long)(maxTimeTaken * 1000);

		public override string ToString()
		{
			return "PingStats{" +
				"ia=" + ia +
				", noPings=" + noPings +
				", packetsLost=" + packetsLost +
				", averageTimeTaken=" + averageTimeTaken +
				", minTimeTaken=" + minTimeTaken +
				", maxTimeTaken=" + maxTimeTaken +
				'}';
		}
	}
}
