using ClientServerCommon;
using ClientServerCommon.Models;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Net.Sockets;
using System.Threading;

namespace ChatBot.Network
{
    public class Client
    {
        #region Properties
        private static ManualResetEvent resetEvent = new ManualResetEvent(false);
        #endregion

        #region Methods

        /// <summary>
        /// Create client socket and connect to IPEndPoint
        /// </summary>
        public void Connect()
        {
            ConnectedObject client = new ConnectedObject();

            client.ClientSocket = NetworkManager.CreateSocket();
            int connectAttempt = 0;

            while (!client.ClientSocket.Connected && connectAttempt < 3)
            {
                try
                {
                    connectAttempt++;
                    Console.WriteLine("Connection attempt " + connectAttempt);

                    // Attempt to connect
                    client.ClientSocket.Connect(NetworkManager.IPEndPoint);
                }
                catch (SocketException ex)
                {
                    Log.Error(ex.Message);
                    Console.Clear();
                }
            }

            // Maximum attempts to connect
            if (!client.ClientSocket.Connected)
            {

                Console.WriteLine("Connect unsuccessful");
                Console.ReadLine();
            }


            Thread sendThread = new Thread(() => Send(client));
            Thread receiveThread = new Thread(() => Receive(client));

            sendThread.Start();
            receiveThread.Start();
        }

        /// <summary>
        /// Send data async to the connected socket
        /// </summary>
        /// <param name="client"></param>
        private static void Send(ConnectedObject client)
        {
            while (true)
            {
                resetEvent.Reset();

                Console.WriteLine("Please Enter your message:");
                client.SetOutgoingMessage(Console.ReadLine());

                byte[] byteMessage = client.ConvertStringToByte(client.OutgoingMessage);

                try
                {
                    client.ClientSocket.BeginSend(byteMessage, 0, byteMessage.Length, SocketFlags.None,
                                                  new AsyncCallback((IAsyncResult ar) => { }), client);

                }
                catch (SocketException ex)
                {
                    Log.Error(ex.Message);
                    client.Disconnect();
                    Thread.CurrentThread.Interrupt();
                    return;
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    Thread.CurrentThread.Interrupt();
                    return;
                }

                resetEvent.WaitOne();
            }
        }

        /// <summary>
        /// Receive data from a bound socket
        /// </summary>
        /// <param name="client"></param>
        private static void Receive(ConnectedObject client)
        {
            int bytesRead = 0;

            while (true)
            {
                try
                {
                    bytesRead = client.ClientSocket.Receive(client.Buffer, SocketFlags.None);
                }
                catch (SocketException ex)
                {
                    Log.Error(ex.Message);
                    client.Disconnect();
                    Console.WriteLine("Server Disconnect");
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }

                if (bytesRead > 0)
                {
                    if (bytesRead > 0)
                    {
                        client.SetIncomingMessage(bytesRead);

                        Console.WriteLine(client.IncomingMessage);

                        bytesRead = 0;

                        resetEvent.Set();
                    }
                }
            }
        }

        #endregion
    }
}
