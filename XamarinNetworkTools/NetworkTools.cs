using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace XamarinNetworkTools
{
    public static class NetworkTools
    {
        public static INetworkTools Instance { get; private set; }

        internal static void Init<TPlatformType>() where TPlatformType : INetworkTools
        {
            try
            {
                Instance = Activator.CreateInstance(typeof(TPlatformType)) as INetworkTools;
            }
            catch (Exception ex)
            {
                throw new NetworkToolsException($"Failed to initialize {typeof(TPlatformType).Name}", ex);
            }
        }
    }
}
