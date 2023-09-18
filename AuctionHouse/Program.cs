using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
IPAddress ipAddr = ipHost.AddressList[0];
IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

try
{
    listener.Bind(localEndPoint);
    listener.Listen(10);
    bool listening = true;

    while (listening)
    {
        Console.WriteLine($"Waiting for connections on {ipAddr}");

        Socket clientSocket = listener.Accept();

        Thread clientThread = new Thread(() =>
        {
            byte[] bytes = new byte[1024];
            string data = null;
            bool connected = true;

            while (connected)
            {
                int numBytes = clientSocket.Receive(bytes);
                data = Encoding.ASCII.GetString(bytes, 0, numBytes);

                if (data.IndexOf("<EOF>") > -1)
                {
                    connected = false;
                }
                else
                {
                    Console.WriteLine("Text received -> {0} ", data);
                    Console.WriteLine("Send your response or stop connection by typing 'stop'");
                    var response = Console.ReadLine();
                    if (response == "stop")
                    {
                        connected = false;
                        byte[] message = Encoding.ASCII.GetBytes("Connection Closed");
                        clientSocket.Send(message);
                    }
                    else
                    {
                        byte[] message = Encoding.ASCII.GetBytes(response);
                        clientSocket.Send(message);
                    }
                }
            }

            clientSocket.Close();
        });

        clientThread.Start();
    }

    listener.Close();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}