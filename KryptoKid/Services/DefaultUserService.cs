using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KryptoKid.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;
using ServiceStack;

namespace KryptoKid.Services
{

    public class DefaultUserService : IUserService
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;

        public DefaultUserService(UserManager<ApplicationUser> userManager, AppDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        // GET THE USER ID FROM THE EMAIL
        public async Task<string> GetUserId(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var userId = user.Id;

            return userId;
        }

        // NEED TO RETRIEVE(GET) PORTFOLIO RECORD QUANTITY
        public int RetrieveCoinQuantity(string email, string stock_name)
        {
            var previousQuantity = 0;
            bool found = false;

            IEnumerable<Portfolio> portfolioRecords = _db.Portfolio.AsEnumerable().Where(b => b.stock_name == stock_name);

            foreach (var entry in portfolioRecords)
            {
                if (entry.email == email)
                {
                    previousQuantity = entry.quantity;
                    found = true;
                }
            }
            if (found == false)
            {
                previousQuantity = 0;
            }

            return previousQuantity;
        }

        // GET BALANCE FROM USER
        public async Task<decimal> GetBalanceFromUser(string email)
        {
            var userBalance = await _userManager.FindByEmailAsync(email);
            var balance = userBalance.Balance;
            return balance;
        }


        // ADD PURCHASE TO THE PURCHASE TABLE
        public void AddPurchaseToDatabase(Purchases purchase)
        {
            _db.Purchases.Add(purchase);
            _db.SaveChanges();
        }


        // ADD SALE TO THE SALE TABLE
        public void AddSaleToDatabase(Sales sale)
        {
            _db.Sales.Add(sale);
            _db.SaveChanges();
        }


        // UPDATE PORTFOLIO ENTRY FOR A GIVEN COIN
        public void UpdatePortfolioEntryForOneCoin(Portfolio portfolio)
        {
            IEnumerable<Portfolio> portfolioRecord = _db.Portfolio.AsEnumerable().Where(b => b.stock_name == portfolio.stock_name);

            foreach (var entry in portfolioRecord)
            {
                if (entry.email == portfolio.email)
                {
                    _db.Portfolio.Remove(entry);
                }
            }

            _db.Portfolio.Add(portfolio);
            _db.SaveChanges();
        }


        // UPDATE USER BALANCE DURING PURCHASE
        public async Task UpdateUserBalancePurchase(string email, decimal price, int quantity)
        {
            var user = await _userManager.FindByEmailAsync(email);
            user.Balance = (user.Balance - (price * quantity));
            await _userManager.UpdateAsync(user);
        }


        // UPDATE USER BALANCE DURING SALE
        public async Task UpdateUserBalanceSale(string email, decimal price, int quantity)
        {
            var user = await _userManager.FindByEmailAsync(email);
            user.Balance = (user.Balance + (price * quantity));
            await _userManager.UpdateAsync(user);
        }

        // GETTING DATA FROM OUR DUMMY DB
        public List<Coins> RetrieveDummyData()
        {

            string[] cryptoArray = new string[5];
            cryptoArray[0] = "BTC";
            cryptoArray[1] = "ETH";
            cryptoArray[2] = "XRP";
            cryptoArray[3] = "BCH";
            cryptoArray[4] = "LTC";

            List<Coins> cryptoList = new List<Coins>();

          
            for (int i = 0; i < cryptoArray.Length; i++)
            {
                Coins cryptoData;
                cryptoData = _db.Coins.AsEnumerable().Where(x => x.From_Currency_Code == cryptoArray[i]).FirstOrDefault();
                cryptoList.Add(cryptoData);
            }

            return cryptoList;
        }

        // GET COINS IN PORTFOLIO
        public List<string[]> CoinsInPortfolio(string email)
        {
            List<Portfolio> portfolio = new List<Portfolio>();
            List<string[]> rows = new List<string[]>();


            portfolio = _db.Portfolio.AsEnumerable().Where(x => x.email == email).ToList();
            foreach (var stock in portfolio)
            {

                string[] literal = new string[3];
                literal[0] = stock.stock_name;

                var price_disposable = portfolio.Where(x => x.stock_name == stock.stock_name).Select(z => z.price);
                literal[1] = price_disposable.First().ToString();

                var quantity_disposable = portfolio.Where(x => x.stock_name == stock.stock_name).Select(z => z.quantity);
                literal[2] = quantity_disposable.First().ToString();

                rows.Add(literal);
            };

            return rows;
        }

        public Coins GetCryptoData(string from_currency)
        {
            // store in an production environment variable later on
            var apiKey = "XHBPMR58X933ZMKL";
            var to_currency = "USD";

            var jsonData = $"https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency={from_currency}&to_currency={to_currency}&apikey={apiKey}".GetStringFromUrl();

            //Debug.WriteLine(jsonData);

            // TODO change file location
            // Relative path to file within the project, probably same directory
            StreamWriter writer = new StreamWriter("./data.json");
            writer.Write(jsonData);
            writer.Close();

            // TODO change file location
            // Relative path to file within the project, probably same directory
            TextReader reader = new StreamReader("./data.json");
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

        public void updateCoinData(Coins coin)
        {
            Coins findEntry = _db.Coins.AsEnumerable().Where(x => x.From_Currency_Code == coin.From_Currency_Code).FirstOrDefault();
            Coins cryptoData = _db.Coins.Find(findEntry.id);
            _db.Coins.Remove(cryptoData);
            _db.Coins.Add(coin);
            _db.SaveChanges();
        }   

        public void DbUpdate()
        {
           
            // INIT ARRAY OF COIN CODES THAT CAN BE SENT TO API METHOD
            Coins cryptoData = new Coins
            {
            };
            List<Coins> cryptoList = new List<Coins>();
            string[] cryptoArray = new string[5];
            cryptoArray[0] = "BTC";
            cryptoArray[1] = "ETH";
            cryptoArray[2] = "XRP";
            cryptoArray[3] = "BCH";
            cryptoArray[4] = "LTC";


            // NEED A WAY OF INCREMENTING OUR INDEX EACH TIME WITHOUT INTIALIZING VARIABLE EACH TIME
            // WILL DO LATER, FOR NOW SINCE WE ONLY HAVE %, IT IS HARDCODED


            // USE CODE AT THAT INDEX AND CALL THE API METHOD AND RECEIVE UP TO DATE COIN VALUES
            cryptoList.Add(GetCryptoData(cryptoArray[0]));

            cryptoList.Add(GetCryptoData(cryptoArray[1]));

            cryptoList.Add(GetCryptoData(cryptoArray[2]));

            cryptoList.Add(GetCryptoData(cryptoArray[3]));

            cryptoList.Add(GetCryptoData(cryptoArray[4]));


            // UPDATE DATABASE ENTRY FOR THAT COIN
            updateCoinData(cryptoList[0]);

            updateCoinData(cryptoList[1]);

            updateCoinData(cryptoList[2]);

            updateCoinData(cryptoList[3]);

            updateCoinData(cryptoList[4]);

        }
    }
}
              

      
 
