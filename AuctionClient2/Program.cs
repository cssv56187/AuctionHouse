using System.Net.Sockets;
using System.Net;
using System.Text;

try
{
    IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
    //IPAddress ipAddr = new IPAddress(new byte[] { 10, 228, 72, 105 });
    IPAddress ipAddr = ipHost.AddressList[0];
    IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
    Socket sender = new Socket(ipAddr.AddressFamily,
               SocketType.Stream, ProtocolType.Tcp);
    sender.Connect(localEndPoint);
    byte[] messageSent = Encoding.ASCII.GetBytes($"bidder: 2 -- has joined the auction!");
    int byteSent = sender.Send(messageSent);
    bool leavingAuctionHouse = false;
    do
    {
        byte[] messageReceived = new byte[1024];
        int byteRecv = sender.Receive(messageReceived);
        Console.WriteLine("Auctioneer ---> {0}",
              Encoding.ASCII.GetString(messageReceived,
                                         0, byteRecv));
        Console.WriteLine("Type 'leave' to leave the auction house or give your bet!!");
        var message = Console.ReadLine();
        if (message == "leave")
        {
            messageSent = Encoding.ASCII.GetBytes($"Bidder 2 is leaving the AuctionHouse<EOF>");
            byteSent = sender.Send(messageSent);
            leavingAuctionHouse = true;
        }
        else
        {
            messageSent = Encoding.ASCII.GetBytes($"{message}");
            byteSent = sender.Send(messageSent);
        }
    }
    while (!leavingAuctionHouse);
    sender.Shutdown(SocketShutdown.Both);
    sender.Close();
}
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
