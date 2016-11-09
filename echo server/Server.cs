using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Collections;
using DataLib;


namespace OurSpiffyServer_ConsoleApp
{
    class Server
    {
        static Socket server;
        static List<ClientData> _clients;
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Server...");
            Console.WriteLine("Enter port:");
            int Port = Convert.ToInt32(Console.ReadLine());
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clients = new List<ClientData>();
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, Port);
            server.Bind(endpoint);
            Thread listenThread = new Thread(ListenThread);
            listenThread.Start();
            Console.WriteLine("Success... Listening Port:" + Port);

        }

        static void ListenThread()
        {
            for (; ; )
            {
                server.Listen(0);
                _clients.Add(new ClientData(server.Accept()));
            }
        }

        public static void Data_IN(object cSocket)
        {
            Socket clientSocket = (Socket)cSocket;
            byte[] Buffer;
            int readBytes;
            for (; ; )
            {
                Buffer = new byte[clientSocket.SendBufferSize];
                try
                {                
                    readBytes = clientSocket.Receive(Buffer);

                    if (readBytes > 0)
                    {
                        Packet p = new Packet(Buffer);
                        dataManager(p);
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine(Buffer[0]+" Disconnected.");
                    clientSocket.Close();
                    break;

                }
            }
        }

        public static void dataManager(Packet p)
        {
            switch (p.PacketType)
            {
                case PacketType.mMessage:
                    foreach (ClientData c in _clients)
                    {
                        if (c.clientThread.IsAlive)
                            c.clientSocket.Send(p.ToBytes());
                    }
                    break;
            }
        }

        class ClientData
        {
            public Socket clientSocket;
            public Thread clientThread;
            public string id;

            public ClientData()
            {
                id = Guid.NewGuid().ToString();
                clientThread = new Thread(Server.Data_IN);
                clientThread.Start(clientSocket);
                sendRegistrationPacket();
            }

            public ClientData(Socket clientSocket)
            {
                this.clientSocket = clientSocket;
                id = Guid.NewGuid().ToString();
                clientThread = new Thread(Server.Data_IN);
                clientThread.Start(clientSocket);
                sendRegistrationPacket();
            }

            public void sendRegistrationPacket()
            {
                Packet p = new Packet(PacketType.name, "server");
                p.Gdata.Add(id);
                clientSocket.Send(p.ToBytes());
            }
        }
    }
}


