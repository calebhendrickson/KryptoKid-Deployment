using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KryptoKid.Models
{
    public class Coins
    {
        public string From_Currency_Code { get; set; }
        public string From_Currency_Name { get; set; }
        public string To_Currency_Code { get; set; }
        public string To_Currency_Name { get; set; }
        public double Exchange_Rate { get; set; }
        public Nullable<System.DateTime> Last_Refreshed { get; set; }
        public string Time_Zone { get; set; }
        public double Bid_price { get; set; }
        public double Ask_price { get; set; }
        public int id { get; set; }
    }
}
