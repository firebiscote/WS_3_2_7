using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    class Client
    {
        static void Main(string[] args)
        {
            Socket cSocket = Connect();
            Thread t1 = new Thread(new ParameterizedThreadStart(Listen));
            Thread t2 = new Thread(new ParameterizedThreadStart(Send));
            t1.Start(cSocket);
            t2.Start(cSocket);
            t2.Join();
            Disconnect(cSocket);
        }

        private static Socket Connect()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, 11000);
            Socket client = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(remoteEP);
            Console.WriteLine($"Connexion à l'IP {ipAddress} par le port {remoteEP.ToString()[(remoteEP.ToString().LastIndexOf(':') + 1)..]}");
            return client;
        }

        private static void Send(object socket)
        {
            Socket client = (Socket)socket;
            string data = "";
            while (data != "quit")
            {
                data = Console.ReadLine();
                byte[] msgB = Encoding.ASCII.GetBytes(data);
                client.Send(msgB);
            }
        }

        private static void Listen(object socket)
        {
            Socket client = (Socket)socket;
            string data;
            while (true)
            {
                byte[] bytes = new byte[1024];
                int bytesRec = client.Receive(bytes);
                data = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (data != "")
                    Console.WriteLine($"Serveur : {data}");
            }
        }

        private static void Disconnect(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
