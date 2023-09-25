using AuctionHouse;
using System.Net;
using System.Net.Sockets;
using System.Text;

IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
IPAddress ipAddr = ipHost.AddressList[0];
IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);
Socket listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
List<Socket> bidders = new List<Socket>();

try
{
    listener.Bind(localEndPoint);
    listener.Listen(10);
    bool auctionHouseIsOpen = true;
    Thread listeningThread = new Thread(() =>
    {
        do
        {
            Console.WriteLine($"Waiting for new bidder...");
            Socket bidder = listener.Accept();
            bidders.Add(bidder);
            Thread clientThread = new Thread(() => Handlers.HandleBidderMessages(bidder));
            clientThread.Start();
        }
        while (auctionHouseIsOpen);

        Console.WriteLine("Doors are closing");
    });
    listeningThread.Start();
   
    Console.WriteLine("To end the Auction, type 'End Auction'");
    do
    {
        var input = Console.ReadLine();
        if(input == "End Auction")
        {
            auctionHouseIsOpen = false;
        }
        else if(input.StartsWith("Start Bid for item:"))
        {
            Handlers.HandleBid(input, bidders);
        }
    }
    while (auctionHouseIsOpen);

    listener.Close();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}





