using ClientServerCommon.Interface;
using System;
using System.Net.Sockets;
using System.Text;

namespace ClientServerCommon.Models
{
    public class ConnectedObject : IConnectedObject
    {

        public readonly int BufferSize = 1024;

        public ConnectedObject()
        {
            Buffer = new byte[BufferSize];
        }

        #region Properties
        public Socket ClientSocket { get; set; }
        public byte[] Buffer { get; set; }
        public string IncomingMessage { get; set; }
        public string OutgoingMessage { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Convert string to byte array
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public byte[] ConvertStringToByte(string message)
        {
            return Encoding.ASCII.GetBytes(message);
        }

        /// <summary>
        /// Convert bytes to string and set property
        /// </summary>
        /// <param name="bytesRead"></param>
        public void SetIncomingMessage(int bytesRead)
        {
            IncomingMessage = Encoding.ASCII.GetString(Buffer, 0, bytesRead);
        }

        /// <summary>
        /// Set property
        /// </summary>
        /// <param name="message"></param>
        public void SetOutgoingMessage(string message)
        {
            OutgoingMessage = message;
        }

        /// <summary>
        /// Disconect send/receive socket
        /// </summary>
        public void Disconnect()
        {
            try
            {
                ClientSocket.Shutdown(SocketShutdown.Both);
                ClientSocket.Close();
            }
            catch (Exception)
            {
                Console.WriteLine("connection already closed");
            }
        }
        #endregion
    }
}
