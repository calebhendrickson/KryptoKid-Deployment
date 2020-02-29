using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KryptoKid.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ServiceStack;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace KryptoKid.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public class ApiCryptoDataController : ControllerBase
    {
        // /api/controllername/key
        // /api/apicryptodata/btc

        [HttpGet]
        public ActionResult<Coins> GetCryptoData(string from_currency)
        {
            // store in an production environment variable later on
            var apiKey = "XHBPMR58X933ZMKL";
            var to_currency = "USD";

            var jsonData = $"https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency={from_currency}&to_currency={to_currency}&apikey={apiKey}".GetStringFromUrl();

            //Debug.WriteLine(jsonData);

            //var monthlyPrices = $"https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol={symbol}&apikey={apiKey}&datatype=csv"
            //                .GetStringFromUrl().FromCsv<List<AlphaVantageData>>();

            StreamWriter writer = new StreamWriter("C:/cryptodata/data.json");
            writer.Write(jsonData);
            writer.Close();

            TextReader reader = new StreamReader("C:/cryptodata/data.json");
            string json = reader.ReadToEnd();
            //Debug.WriteLine(json);
            var jsonObject = JObject.Parse(json);

            Coins ourResult = new Coins
            {
                From_Currency_Code = jsonObject["Realtime Currency Exchange Rate"]["1. From_Currency Code"].ToString(),
                From_Currency_Name = jsonObject["Realtime Currency Exchange Rate"]["2. From_Currency Name"].ToString(),
                To_Currency_Code = jsonObject["Realtime Currency Exchange Rate"]["3. To_Currency Code"].ToString(),
                To_Currency_Name = jsonObject["Realtime Currency Exchange Rate"]["4. To_Currency Name"].ToString(),
                Exchange_Rate = Convert.ToDouble(jsonObject["Realtime Currency Exchange Rate"]["5. Exchange Rate"].ToString()),

                // check on this
                Last_Refreshed = Convert.ToDateTime(jsonObject["Realtime Currency Exchange Rate"]["6. Last Refreshed"].ToString()),
                Time_Zone = jsonObject["Realtime Currency Exchange Rate"]["7. Time Zone"].ToString(),
                Bid_price = Convert.ToDouble(jsonObject["Realtime Currency Exchange Rate"]["8. Bid Price"].ToString()),
                Ask_price = Convert.ToDouble(jsonObject["Realtime Currency Exchange Rate"]["9. Ask Price"].ToString()),
            };

            reader.Close();

            return ourResult;

        }
    }
}