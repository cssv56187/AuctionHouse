using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Runtime.CompilerServices;
using System.Reflection;

bool runs = true;
do
{
    try
    {
        // Establish the remote endpoint
        // for the socket. This example
        // uses port 11111 on the local
        // computer.
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        //IPAddress ipAddr = new IPAddress(new byte[] { 10, 228, 72, 105 });
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

        // Creation TCP/IP Socket using
        // Socket Class Constructor
        Socket sender = new Socket(ipAddr.AddressFamily,
                   SocketType.Stream, ProtocolType.Tcp);

        // Connect Socket to the remote
        // endpoint using method Connect()
        sender.Connect(localEndPoint);

        // We print EndPoint information
        // that we are connected
        Console.WriteLine("Socket connected to -> {0} ",
                      sender.RemoteEndPoint.ToString());

        // Creation of message that
        // we will send to Server
        byte[] messageSent = Encoding.ASCII.GetBytes($"Client {ipHost.HostName} -- client: 2 -- has joined the auction :D");
        int byteSent = sender.Send(messageSent);

        bool inAuctionhouse = true;
        do
        {
            // Data buffer
            byte[] messageReceived = new byte[1024];

            // We receive the message using
            // the method Receive(). This
            // method returns number of bytes
            // received, that we'll use to
            // convert them to string
            int byteRecv = sender.Receive(messageReceived);
            Console.WriteLine("Auctionhouse -> {0}",
                  Encoding.ASCII.GetString(messageReceived,
                                             0, byteRecv));

            Console.WriteLine("Type 'leave' to leave the auction house or give your bet!!");
            var message = Console.ReadLine();
            if (message == "leave")
            {
                messageSent = Encoding.ASCII.GetBytes($"Client has left the Auction<EOF>");
                byteSent = sender.Send(messageSent);
                inAuctionhouse = false;
            }
            else
            {
                messageSent = Encoding.ASCII.GetBytes($"{message}");
                byteSent = sender.Send(messageSent);
            }
        }
        while (inAuctionhouse);

        // Close Socket using
        // the method Close()
        sender.Shutdown(SocketShutdown.Both);
        sender.Close();
    }

    // Manage of Socket's Exceptions
    catch (ArgumentNullException ane)
    {

        Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
    }

    catch (SocketException se)
    {

        Console.WriteLine("SocketException : {0}", se.ToString());
    }

    catch (Exception e)
    {
        Console.WriteLine("Unexpected exception : {0}", e.ToString());
    }


    Console.WriteLine("Skriv {stop} for at afslutte programmet");
    while (runs)
    {
        if (Console.ReadLine() == "stop")
        {
            runs = false;
        }
    }
}
while (runs);
