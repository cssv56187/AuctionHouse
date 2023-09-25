using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouse
{
    public static class Handlers
    {
        public static void HandleBidderMessages(Socket bidder)
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
        }

        public static void HandleBid(string input, List<Socket> bidders)
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
}
