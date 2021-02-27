using ClientServerCommon.Interface;
using System;
using System.Net.Sockets;

namespace ClientServerCommon.Models
{
    public class ServerNetworkManager : NetworkManager, IServerNetworkManager
    {
        #region Methods

        /// <summary>
        /// Create new instance of server socket
        /// </summary>
        /// <returns></returns>
        public static Socket StartListening()
        {
            Socket socket = null;

            try
            {
                socket = CreateSocket();
                socket.Bind(IPEndPoint);
                socket.Listen(100);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return socket;

        }
        #endregion
    }
}
