using ClientServerCommon.Interface;
using System.Net;
using System.Net.Sockets;

namespace ClientServerCommon.Models
{
    public class NetworkManager : INetworkManager
    {
        #region Properties
        private static IPAddress IPAddress { get => IPAddress.Loopback; }
        private static int Port { get => 3000; }
     
        // End Point For Client Connection
        public static IPEndPoint IPEndPoint { get => new IPEndPoint(IPAddress, Port); }
        #endregion

        #region Methods

        /// <summary>
        /// Create new instance of socket
        /// </summary>
        /// <returns></returns>
        public static Socket CreateSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
        #endregion
    }
}
