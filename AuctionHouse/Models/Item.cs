using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouse.Models
{
    public class Item
    {
        public string Name { get; set; }
        public decimal MinPrice { get; set; }
        public decimal OriginalPrice { get; set; }
    }
}
