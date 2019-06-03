using System;
namespace XamarinNetworkTools
{
    public class NetworkToolsException : System.Exception
    {
        public NetworkToolsException()
        {
        }

        public NetworkToolsException(string message) : base(message) {  }

        public NetworkToolsException(string message, Exception inner) : base(message, inner) { }
    }
}
