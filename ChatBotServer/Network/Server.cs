using ChatBotServer.Interface;
using ClientServerCommon.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace ChatBotServer.Network
{
    public class Server : IServer
    {
        private static ManualResetEvent resetEvent = new ManualResetEvent(false);

        #region Properties
        public static List<ConnectedObject> Clients { get; set; }
        private static Socket ServerSocket { get; set; } = null;
        #endregion

        #region Methods

        /// <summary>
        /// Create socket instance and listen to connections
        /// </summary>
        public static void StartListening()
        {
            try
            {
                Clients = new List<ConnectedObject>();

                ServerSocket = ServerNetworkManager.StartListening();
                Console.WriteLine($"Server running..");

                while (true)
                {
                    resetEvent.Reset();

                    // Start an asynchronous socket to listen for connections
                    ServerSocket.BeginAccept(new AsyncCallback(HandleClientConnect), ServerSocket);

                    // Wait until connection is made.
                    resetEvent.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Console.WriteLine("Ooops, Somthing Wrong..");
            }
        }

        /// <summary>
        /// Callback for handling client connection
        /// </summary>
        /// <param name="ar"></param>
        private static void HandleClientConnect(IAsyncResult ar)
        {
            Console.WriteLine("New Connection");

            resetEvent.Set();

            try
            {
                Socket clientSocket = ServerSocket.EndAccept(ar);

                ConnectedObject client = new ConnectedObject()
                {
                    ClientSocket = clientSocket
                };

                Clients.Add(client);

                ListenForIncomingMessages(client);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }

        /// <summary>
        /// Begin receive data from connected socket
        /// </summary>
        /// <param name="client"></param>
        private static void ListenForIncomingMessages(ConnectedObject client)
        {
            try
            {
                client.ClientSocket.BeginReceive(client.Buffer, 0, client.BufferSize, SocketFlags.None, new AsyncCallback(ReceiveMessages), client);
            }
            catch (SocketException ex)
            {
                Log.Error(ex.Message);
                CloseClientSocket(client);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Callback for receiving messages
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveMessages(IAsyncResult ar)
        {
            ConnectedObject client;
            int readBytes = 0;
            if (!GetClientFromCallback(ar, out client)) return;

            try
            {
                readBytes = client.ClientSocket.EndReceive(ar);
            }
            catch (SocketException ex)
            {
                CloseClientSocket(client);
                Log.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }

            if(readBytes > 0)
            {
                client.SetIncomingMessage(readBytes);

                Console.WriteLine($"Client message recived: {client.IncomingMessage}");

                SendReply(client);

                ListenForIncomingMessages(client);
            }
        }

        /// <summary>
        /// Get client object from callback
        /// </summary>
        /// <param name="ar"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private static bool GetClientFromCallback(IAsyncResult ar, out ConnectedObject client)
        {
            client = null;

            if (ar != null)
            {
                try
                {
                    client = (ConnectedObject)ar.AsyncState;
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }

            Console.WriteLine("Ooops, Connection problem.");
            return false;
        }

        /// <summary>
        /// Send async data to connected socket
        /// </summary>
        /// <param name="client"></param>
        private static void SendReply(ConnectedObject client)
        {
            if (client == null)
            {
                Console.WriteLine("Ooops, Connection problem.");
                return;
            }

            Console.WriteLine("Proccessing client message");
            // TODO function the reverse client message ReverseMessage()
            string returnMessage = ReverseMessage(client.IncomingMessage);

            client.SetOutgoingMessage(returnMessage);

            var byteReply = client.ConvertStringToByte(client.OutgoingMessage);

            try
            {
                client.ClientSocket.BeginSend(byteReply, 0, byteReply.Length, SocketFlags.None, 
                                              new AsyncCallback((IAsyncResult ar) => 
                                              { Console.WriteLine("Reply Sent!"); }), client);
            }
            catch (SocketException ex)
            {
                // Client was forcebly closed on the client side
                CloseClientSocket(client);
                Log.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
        
        /// <summary>
        /// Reverse message returned to client
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string ReverseMessage(string message)
        {
            if (message.Length == 1)
                return message;

            return message[message.Length - 1] + ReverseMessage(message.Substring(0, message.Length - 1));
        }

        /// <summary>
        /// Close socket send/receive
        /// </summary>
        /// <param name="client"></param>
        private static void CloseClientSocket(ConnectedObject client)
        {
            Console.WriteLine("Client disconnected");
            client.Disconnect();
            if (Clients.Contains(client))
            {
                Clients.Remove(client);
            }
        }

        /// <summary>
        /// Close all connected socket send/receive
        /// </summary>
        public static void CloseAllSockets()
        {
            // Close all clients
            foreach (var client in Clients)
            {
                client.Disconnect();
            }
            // Close server
            ServerSocket.Close();
        }
        #endregion
    }
}
