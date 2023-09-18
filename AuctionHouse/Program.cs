using System.Net;
using System.Net.Sockets;
using System.Text;

IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
IPAddress ipAddr = ipHost.AddressList[0];
IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
try
{
    listener.Bind(localEndPoint);
    listener.Listen(10);
    bool doorsAreOpen = true;
    do
    {
        Console.WriteLine($"Waiting for new bidder...");
        // new bidder has entered
        Socket clientSocket = listener.Accept();
        Thread clientThread = new Thread(() =>
        {
            byte[] bytes = new byte[1024];
            string data = null;
            bool bidderIsLeaving = false;
            do
            {
                int numBytes = clientSocket.Receive(bytes);
                data = Encoding.ASCII.GetString(bytes, 0, numBytes);
                if (data.IndexOf("<EOF>") > -1)
                {
                    bidderIsLeaving = false;
                }
                else
                {
                    Console.WriteLine("Text received -> {0} ", data);
                    
                }
            }
            while (!bidderIsLeaving);
            clientSocket.Close();
        });
        clientThread.Start();
    }
    while (doorsAreOpen);


    //Console.WriteLine("Send your response or stop connection by typing 'stop'");
    //var response = Console.ReadLine();
    //if (response == "stop")
    //{
    //    bidderIsLeaving = false;
    //    byte[] message = Encoding.ASCII.GetBytes("Connection Closed");
    //    clientSocket.Send(message);
    //}
    //else
    //{
    //    byte[] message = Encoding.ASCII.GetBytes(response);
    //    clientSocket.Send(message);
    //}

    listener.Close();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}