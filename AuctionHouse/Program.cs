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
            // new bidder has entered
            Socket bidder = listener.Accept();
            bidders.Add(bidder);
            Thread clientThread = new Thread(() =>
            {
                byte[] bytes = new byte[1024];
                string data = null;
                bool bidderIsLeaving = false;
                do
                {
                    int numBytes = bidder.Receive(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, numBytes);
                    if (data.IndexOf("<EOF>") > -1)
                    {
                        bidderIsLeaving = false;
                    }
                    else if (data.StartsWith("Bid"))
                    {
                        data.Split(':');
                        IPEndPoint clientEndPoint = (IPEndPoint)bidder.RemoteEndPoint;
                        string clientIPAddress = clientEndPoint.Address.ToString();
                        Console.WriteLine($"{clientIPAddress} Bid {0} {data[1]}");
                    }
                    else
                    {
                        Console.WriteLine("Text received -> {0} ", data);

                    }
                }
                while (!bidderIsLeaving);
                bidder.Close();
            });
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
            byte[] message = Encoding.ASCII.GetBytes(input);
            foreach (var bidder in bidders)
            {
                bidder.Send(message);
            }
            for (int i = 5; i > 0; i--)
            {
                foreach (var bidder in bidders)
                {
                    byte[] timeLeft = Encoding.ASCII.GetBytes($"Time left to bid: {i} seconds");
                    bidder.Send(timeLeft);
                }
                Console.WriteLine($"Time left to bid: {i} seconds");
                Thread.Sleep(1000);
            }
            string itemName = input.Split(':')[1];
            byte[] bidDone = Encoding.ASCII.GetBytes($"Bid done for item: {itemName}");
            Console.WriteLine($"Bid done for item: {itemName}");
            foreach (var bidder in bidders)
            {
                bidder.Send(bidDone);
            }
        }
    }
    while (auctionHouseIsOpen);

    listener.Close();
}
catch (Exception e)
{
    Console.WriteLine(e.ToString());
}

