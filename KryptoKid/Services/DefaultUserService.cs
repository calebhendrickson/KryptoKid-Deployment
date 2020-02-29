using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KryptoKid.Models;
using Microsoft.AspNetCore.Identity;

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
    }
}
              

      
 
