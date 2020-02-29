using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DayTrader.Models
{

    public class Crypto
    {
        //[Column("From_Currency Code")]
        public string From_Currency_Code { get; set; } // ex: "BTC"

        //[Column("From_Currency Name")]
        public string From_Currency_Name { get; set; } //ex: "Bitcoin"

        //[Column("To_Currency Code")]
        public string To_Currency_Code { get; set; } // ex: "CNY",

        //[Column("To_Currency Name")]
        public string To_Currency_Name { get; set; } // ex: "Chinese Yuan",

        //[Column("Exchange Rate")]
        public double Exchange_Rate { get; set; } // ex: "68082.78640600",

        // check on this
        //[Column("Last Refreshed")]
        public DateTime Last_Refreshed { get; set; } // ex: "2020-02-21 21:07:05",

        //[Column("Time Zone")]
        public string Time_Zone { get; set; } //  ex: "UTC",

        //[Column("Bid Price")]
        public double Bid_Price { get; set; } // ex: "68082.78640600",

        //[Column("Ask Price")]
        public double Ask_Price { get; set; } // ex: "68082.85668000"
    }
}
