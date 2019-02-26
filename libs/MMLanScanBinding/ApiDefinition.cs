using System;

using UIKit;
using Foundation;
using ObjCRuntime;
using CoreGraphics;
//using MMLanScan;

namespace MMLanScanBinding
{

	// @interface MacFinder : NSObject
	[BaseType(typeof(NSObject))]
	interface MacFinder
	{
		// +(NSString *)ip2mac:(NSString *)strIP;
		[Static]
		[Export("ip2mac:")]
		string Ip2mac(string strIP);
	}

	// @interface SimplePing : NSObject
	[BaseType(typeof(NSObject))]
	interface SimplePing
	{
		// +(SimplePing *)simplePingWithHostName:(NSString *)hostName;
		[Static]
		[Export("simplePingWithHostName:")]
		SimplePing SimplePingWithHostName(string hostName);

		// +(SimplePing *)simplePingWithHostAddress:(NSData *)hostAddress;
		[Static]
		[Export("simplePingWithHostAddress:")]
		SimplePing SimplePingWithHostAddress(NSData hostAddress);

		[Wrap("WeakDelegate")]
		SimplePingDelegate Delegate { get; set; }

		// @property (readwrite, nonatomic, weak) id<SimplePingDelegate> delegate;
		[NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		// @property (readonly, copy, nonatomic) NSString * hostName;
		[Export("hostName")]
		string HostName { get; }

		// @property (readonly, copy, nonatomic) NSData * hostAddress;
		[Export("hostAddress", ArgumentSemantic.Copy)]
		NSData HostAddress { get; }

		// @property (readonly, assign, nonatomic) uint16_t identifier;
		[Export("identifier")]
		ushort Identifier { get; }

		// @property (readonly, assign, nonatomic) uint16_t nextSequenceNumber;
		[Export("nextSequenceNumber")]
		ushort NextSequenceNumber { get; }

		// -(void)start;
		[Export("start")]
		void Start();

		// -(void)sendPingWithData:(NSData *)data;
		[Export("sendPingWithData:")]
		void SendPingWithData(NSData data);

		// -(void)stop;
		[Export("stop")]
		void Stop();

		// +(const struct ICMPHeader *)icmpInPacket:(NSData *)packet;
		//[Static]
		//[Export("icmpInPacket:")]
		//unsafe ICMPHeader* IcmpInPacket(NSData packet);
	}

	// @protocol SimplePingDelegate <NSObject>
	[Protocol, Model]
	[BaseType(typeof(NSObject))]
	interface SimplePingDelegate
	{
		// @optional -(void)simplePing:(SimplePing *)pinger didStartWithAddress:(NSData *)address;
		[Export("simplePing:didStartWithAddress:")]
		void DidStartWithAddress(SimplePing pinger, NSData address);

		// @optional -(void)simplePing:(SimplePing *)pinger didFailWithError:(NSError *)error;
		[Export("simplePing:didFailWithError:")]
		void DidFailWithError(SimplePing pinger, NSError error);

		// @optional -(void)simplePing:(SimplePing *)pinger didSendPacket:(NSData *)packet;
		[Export("simplePing:didSendPacket:")]
		void DidSendPacket(SimplePing pinger, NSData packet);

		// @optional -(void)simplePing:(SimplePing *)pinger didFailToSendPacket:(NSData *)packet error:(NSError *)error;
		[Export("simplePing:didFailToSendPacket:error:")]
		void DidFailToSendPacket(SimplePing pinger, NSData packet, NSError error);

		// @optional -(void)simplePing:(SimplePing *)pinger didReceivePingResponsePacket:(NSData *)packet;
		[Export("simplePing:didReceivePingResponsePacket:")]
		void DidReceivePingResponsePacket(SimplePing pinger, NSData packet);

		// @optional -(void)simplePing:(SimplePing *)pinger didReceiveUnexpectedPacket:(NSData *)packet;
		[Export("simplePing:didReceiveUnexpectedPacket:")]
		void DidReceiveUnexpectedPacket(SimplePing pinger, NSData packet);
	}

	// @interface LANProperties : NSObject
	[BaseType(typeof(NSObject))]
	interface LANProperties
	{
		// +(MMDevice *)localIPAddress;
		[Static]
		[Export("localIPAddress")]
		//[Verify(MethodToProperty)]
		MMDevice LocalIPAddress { get; }

		// +(NSString *)getHostFromIPAddress:(NSString *)ipAddress;
		[Static]
		[Export("getHostFromIPAddress:")]
		string GetHostFromIPAddress(string ipAddress);

		// +(NSArray *)getAllHostsForIP:(NSString *)ipAddress andSubnet:(NSString *)subnetMask;
		[Static]
		[Export("getAllHostsForIP:andSubnet:")]
		//[Verify(StronglyTypedNSArray)]
		NSObject[] GetAllHostsForIP(string ipAddress, string subnetMask);

		// +(NSString *)fetchSSIDInfo;
		[Static]
		[Export("fetchSSIDInfo")]
		//[Verify(MethodToProperty)]
		string FetchSSIDInfo { get; }
	}

	// @interface MACOperation : NSOperation
	[BaseType(typeof(NSOperation))]
	interface MACOperation
	{
		// -(instancetype _Nullable)initWithIPToRetrieveMAC:(NSString * _Nonnull)ip andBrandDictionary:(NSDictionary * _Nullable)brandDictionary andCompletionHandler:(void (^ _Nullable)(NSError * _Nullable, NSString * _Nonnull, MMDevice * _Nonnull))result;
		[Export("initWithIPToRetrieveMAC:andBrandDictionary:andCompletionHandler:")]
		IntPtr Constructor(string ip, [NullAllowed] NSDictionary brandDictionary, [NullAllowed] Action<NSError, NSString, MMDevice> result);
	}

	// @protocol MMLANScannerDelegate <NSObject>
	[Protocol, Model]
	[BaseType(typeof(NSObject))]
	interface MMLANScannerDelegate
	{
		// @required -(void)lanScanDidFindNewDevice:(MMDevice *)device;
		[Abstract]
		[Export("lanScanDidFindNewDevice:")]
		void LanScanDidFindNewDevice(MMDevice device);

		// @required -(void)lanScanDidFinishScanningWithStatus:(MMLanScannerStatus)status;
		[Abstract]
		[Export("lanScanDidFinishScanningWithStatus:")]
		void LanScanDidFinishScanningWithStatus(MMLanScannerStatus status);

		// @required -(void)lanScanDidFailedToScan;
		[Abstract]
		[Export("lanScanDidFailedToScan")]
		void LanScanDidFailedToScan();

		// @optional -(void)lanScanProgressPinged:(float)pingedHosts from:(NSInteger)overallHosts;
		[Export("lanScanProgressPinged:from:")]
		void LanScanProgressPinged(float pingedHosts, nint overallHosts);
	}

	// @interface MMLANScanner : NSObject
	[BaseType(typeof(NSObject))]
	interface MMLANScanner
	{
		[Wrap("WeakDelegate")]
		MMLANScannerDelegate Delegate { get; set; }

		// @property (nonatomic, weak) id<MMLANScannerDelegate> delegate;
		[NullAllowed, Export("delegate", ArgumentSemantic.Weak)]
		NSObject WeakDelegate { get; set; }

		// -(instancetype)initWithDelegate:(id<MMLANScannerDelegate>)delegate;
		[Export("initWithDelegate:")]
		IntPtr Constructor(MMLANScannerDelegate @delegate);

		// @property (readonly, assign, nonatomic) BOOL isScanning;
		[Export("isScanning")]
		bool IsScanning { get; }

		// -(void)start;
		[Export("start")]
		void Start();

		// -(void)stop;
		[Export("stop")]
		void Stop();
	}

	// @interface NetworkCalculator : NSObject
	[BaseType(typeof(NSObject))]
	interface NetworkCalculator
	{
		// +(NSArray *)getAllHostsForIP:(NSString *)ipAddress andSubnet:(NSString *)subnetMask;
		[Static]
		[Export("getAllHostsForIP:andSubnet:")]
		//[Verify(StronglyTypedNSArray)]
		NSObject[] GetAllHostsForIP(string ipAddress, string subnetMask);
	}

	// @interface PingOperation : NSOperation <SimplePingDelegate>
	[BaseType(typeof(NSOperation))]
	interface PingOperation : SimplePingDelegate //ISimplePingDelegate
	{
		// -(instancetype _Nullable)initWithIPToPing:(NSString * _Nonnull)ip andCompletionHandler:(void (^ _Nullable)(NSError * _Nullable, NSString * _Nonnull))result;
		[Export("initWithIPToPing:andCompletionHandler:")]
		IntPtr Constructor(string ip, [NullAllowed] Action<NSError, NSString> result);
	}

	// @interface MMDevice : NSObject
	[BaseType(typeof(NSObject))]
	interface MMDevice
	{
		// @property (nonatomic, strong) NSString * hostname;
		[Export("hostname", ArgumentSemantic.Strong)]
		string Hostname { get; set; }

		// @property (nonatomic, strong) NSString * ipAddress;
		[Export("ipAddress", ArgumentSemantic.Strong)]
		string IpAddress { get; set; }

		// @property (nonatomic, strong) NSString * macAddress;
		[Export("macAddress", ArgumentSemantic.Strong)]
		string MacAddress { get; set; }

		// @property (nonatomic, strong) NSString * subnetMask;
		[Export("subnetMask", ArgumentSemantic.Strong)]
		string SubnetMask { get; set; }

		// @property (nonatomic, strong) NSString * brand;
		[Export("brand", ArgumentSemantic.Strong)]
		string Brand { get; set; }

		// @property (assign, nonatomic) BOOL isLocalDevice;
		[Export("isLocalDevice")]
		bool IsLocalDevice { get; set; }

		// -(NSString *)macAddressLabel;
		[Export("macAddressLabel")]
		//[Verify(MethodToProperty)]
		string MacAddressLabel { get; }
	}

	[Static]
	//[Verify(ConstantsInterfaceAssociation)]
	partial interface Constants
	{
		// extern double MMLanScanVersionNumber;
		[Field("MMLanScanVersionNumber", "__Internal")]
		double MMLanScanVersionNumber { get; }

		// extern const unsigned char [] MMLanScanVersionString;
		[Field("MMLanScanVersionString", "__Internal")]
		NSString MMLanScanVersionString { get; }
	}

}
