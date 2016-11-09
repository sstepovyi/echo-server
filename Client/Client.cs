using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using DataLib;
using System.Diagnostics;


namespace Client
{

    class Client
    {
        public static Socket cMaster;
        public static string name;
        public static string id;

        static void Main(string[] args)
        {
            try {
                ConsoleKeyInfo cki;
                Console.WriteLine("Enter Your Name:");
                name = Console.ReadLine();
                Console.Clear();
                cMaster = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                InitializeConnection();
                Thread t = new Thread(data_IN);
                t.Start();

                for (;;)
                {
                    Console.Write("::>");

                    cki = Console.ReadKey();
                    string input = Console.ReadLine();
                    Packet p = new Packet(PacketType.mMessage, id);
                    p.Gdata.Add(name);
                    p.Gdata.Add(cki.KeyChar + input);
                    cMaster.Send(p.ToBytes());

                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }


        }

        static void InitializeConnection()
        {
            Console.WriteLine("Please choose server :\n127.0.0.1 \n10.155.133.21\n10.155.133.13");
            Console.WriteLine();
            string ServerIp = Console.ReadLine();
            Console.WriteLine("Please choose port :\n9070 \n9080\n9090");
            Console.WriteLine();
            int ServerPort = Convert.ToInt32(Console.ReadLine());

            try
            {
                serverConnect.ServerList(ServerIp, ServerPort);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not connect to server..");
                Thread.Sleep(100);
                InitializeConnection();
            }


        }

        static void data_IN()
        {
            byte[] buffer;
            int readBytes;

            for (; ; )
            {
                try
                {
                    buffer = new byte[cMaster.SendBufferSize];
                    readBytes = cMaster.Receive(buffer);
                    if (readBytes > 0)
                    {
                        DataManager(new Packet(buffer));
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Server Lost!..Reconecting to another server");
                    cMaster.Disconnect(true);
                    serverConnect.ServerList();
                }
            }
        }
        static void DataManager(Packet p)
        {
            switch (p.PacketType)
            {
                case PacketType.name:
                    Console.WriteLine("Connected to Server!");
                    id = p.Gdata[0];
                    Console.Write("::>");
                    break;
                case PacketType.mMessage:
                    if (p.Gdata[0] != name)
                    { 
                    Console.WriteLine(p.Gdata[0] + ": " + p.Gdata[1]);
                    
                    }
                    else
                    {  }
                    break;
                  
            }
        }
        class serverConnect
        {
            static List<sLists> servers;

            public static List<sLists> Getservlist()
            {
                servers = new List<sLists>
                {
                 new sLists() {sPort=9070,sIp="127.0.0.1",sNumber=1},
                 new sLists() {sPort=9080,sIp="127.0.0.1",sNumber=2},
                 new sLists() {sPort=9090,sIp="10.155.133.11",sNumber=3},                 
                };
                return servers;
            }

            public static void ServerList()
            {
                var lservers = Getservlist();
                var enservers = lservers.ToArray();
                int i =0;

                foreach (sLists l in lservers)
                {
                    try
                    {
                        ServerList(l.sIp, l.sPort);
                        Console.WriteLine("Reconnected to server " + l.sPort);
                        i = 1;
                        break;
                    }
                    catch
                    {
                        Console.Write(".....");
                        i = 0;
                        continue;
                    }
                }
                if ( i!=1)
                {
                    Console.WriteLine("No Active servers avaliable..Please Close The program");
                    Console.ReadLine();
                }
            }

            public static void ServerList(string ServerIp, int ServerPort)
            {
                string dServer = ServerIp;
                int dPort = ServerPort;
                cMaster = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint rEndPoint = new IPEndPoint(IPAddress.Parse(dServer), dPort);
                cMaster.Connect(rEndPoint);
            }
        }
    }

    public class sLists
    {
        public int sPort { get; set; }
        public string sIp { get; set; }
        public int sNumber { get; set; }


    }


}