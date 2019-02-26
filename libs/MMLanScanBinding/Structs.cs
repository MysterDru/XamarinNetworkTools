using System.Runtime.InteropServices;
//using Darwin;
//using Dar

namespace MMLanScanBinding
{
	//[StructLayout (LayoutKind.Sequential)]
	//public struct rt_metrics
	//{
	//	public uint rmx_locks;

	//	public uint rmx_mtu;

	//	public uint rmx_hopcount;

	//	public int rmx_expire;

	//	public uint rmx_recvpipe;

	//	public uint rmx_sendpipe;

	//	public uint rmx_ssthresh;

	//	public uint rmx_rtt;

	//	public uint rmx_rttvar;

	//	public uint rmx_pksent;

	//	public uint[] rmx_filler;
	//}

	//[StructLayout (LayoutKind.Sequential)]
	//public struct rtstat
	//{
	//	public short rts_badredirect;

	//	public short rts_dynamic;

	//	public short rts_newgateway;

	//	public short rts_unreach;

	//	public short rts_wildcard;
	//}

	//[StructLayout (LayoutKind.Sequential)]
	//public struct rt_msghdr
	//{
	//	public ushort rtm_msglen;

	//	public byte rtm_version;

	//	public byte rtm_type;

	//	public ushort rtm_index;

	//	public int rtm_flags;

	//	public int rtm_addrs;

	//	public int rtm_pid;

	//	public int rtm_seq;

	//	public int rtm_errno;

	//	public int rtm_use;

	//	public uint rtm_inits;

	//	public rt_metrics rtm_rmx;
	//}

	//[StructLayout (LayoutKind.Sequential)]
	//public struct rt_msghdr2
	//{
	//	public ushort rtm_msglen;

	//	public byte rtm_version;

	//	public byte rtm_type;

	//	public ushort rtm_index;

	//	public int rtm_flags;

	//	public int rtm_addrs;

	//	public int rtm_refcnt;

	//	public int rtm_parentflags;

	//	public int rtm_reserved;

	//	public int rtm_use;

	//	public uint rtm_inits;

	//	public rt_metrics rtm_rmx;
	//}

	//[StructLayout (LayoutKind.Sequential)]
	//public struct rt_addrinfo
	//{
	//	public int rti_addrs;

	//	public unsafe sockaddr*[] rti_info;
	//}

	//[StructLayout (LayoutKind.Sequential)]
	//public struct arphdr
	//{
	//	public ushort ar_hrd;

	//	public ushort ar_pro;

	//	public byte ar_hln;

	//	public byte ar_pln;

	//	public ushort ar_op;
	//}

	//[StructLayout (LayoutKind.Sequential)]
	//public struct arpreq
	//{
	//	public sockaddr arp_pa;

	//	public sockaddr arp_ha;

	//	public int arp_flags;
	//}

	//[StructLayout (LayoutKind.Sequential)]
	//public struct arpstat
	//{
	//	public uint txrequests;

	//	public uint txreplies;

	//	public uint txannounces;

	//	public uint rxrequests;

	//	public uint rxreplies;

	//	public uint received;

	//	public uint txconflicts;

	//	public uint invalidreqs;

	//	public uint reqnobufs;

	//	public uint dropped;

	//	public uint purged;

	//	public uint timeouts;

	//	public uint dupips;

	//	public uint inuse;

	//	public uint txurequests;
	//}

	//[StructLayout (LayoutKind.Sequential)]
	//public struct ether_arp
	//{
	//	public arphdr ea_hdr;

	//	public byte[] arp_spa;

	//	public byte[] arp_tpa;
	//}

	//[StructLayout (LayoutKind.Sequential)]
	//public struct sockaddr_inarp
	//{
	//	public byte sin_len;

	//	public byte sin_family;

	//	public ushort sin_port;

	//	public in_addr sin_addr;

	//	public in_addr sin_srcaddr;

	//	public ushort sin_tos;

	//	public ushort sin_other;
	//}

	[StructLayout (LayoutKind.Sequential)]
	public struct IPHeader
	{
		public byte versionAndHeaderLength;

		public byte differentiatedServices;

		public ushort totalLength;

		public ushort identification;

		public ushort flagsAndFragmentOffset;

		public byte timeToLive;

		public byte protocol;

		public ushort headerChecksum;

		public byte[] sourceAddress;

		public byte[] destinationAddress;
	}

	//[Verify (InferredFromMemberPrefix)]
	public enum kICMPTypeEchoRe : uint
	{
		ply = 0,
		quest = 8
	}

	[StructLayout (LayoutKind.Sequential)]
	public struct ICMPHeader
	{
		public byte type;

		public byte code;

		public ushort checksum;

		public ushort identifier;

		public ushort sequenceNumber;
	}

	public enum MMLanScannerStatus : uint
	{
		Finished,
		Cancelled
	}
}