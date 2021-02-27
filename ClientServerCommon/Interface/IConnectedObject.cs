using System.Net.Sockets;

namespace ClientServerCommon.Interface
{
    public interface IConnectedObject
    {
        Socket ClientSocket { get; set; }
        byte[] Buffer { get; set; }
        string IncomingMessage { get; set; }
        string OutgoingMessage { get; set; }

        byte[] ConvertStringToByte(string message);
        void SetIncomingMessage(int bytesRead);
        void SetOutgoingMessage(string message);
        void Disconnect();
    }
}
