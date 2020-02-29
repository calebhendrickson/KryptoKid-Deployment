using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KryptoKid.Models
{
    public class Portfolio
    {
        public int id { get; set; }
        public string stock_name { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string email { get; set; }
        public string userid { get; set; }
    }
}
