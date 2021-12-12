using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            Socket cSocket = Connect();
            Socket aSocket = Accept(cSocket);
            Thread t1 = new Thread(new ParameterizedThreadStart(Listen));
            Thread t2 = new Thread(new ParameterizedThreadStart(Send));
            t1.Start(aSocket);
            t2.Start(aSocket);
            t2.Join();
            Disconnect(aSocket);
        }

        private static Socket Connect()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(100);
            return listener;
        }

        private static Socket Accept(Socket socket)
        {
            Console.WriteLine($"Connexion du client : {socket}");
            return socket.Accept();
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
                    Console.WriteLine($"Client : {data}");
            }
        }

        private static void Disconnect(Socket socket)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }
}
