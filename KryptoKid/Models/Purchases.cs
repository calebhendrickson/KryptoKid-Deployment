using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KryptoKid.Models
{
    public class Purchases
    {
        public int id { get; set; }
        public string userid { get; set; }
        public string stock_name { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<int> quantity { get; set; }
    }
}
