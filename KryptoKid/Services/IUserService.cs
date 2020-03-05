using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KryptoKid.Models;

namespace KryptoKid.Services
{
    public interface IUserService
    {

        // GET THE USER ID FROM THE EMAIL
        Task<string> GetUserId(string email);
        

        // NEED TO RETRIEVE(GET) PORTFOLIO RECORD QUANTITY
        int RetrieveCoinQuantity(string email, string stock_name);
  

        // GET BALANCE FROM USERS
        Task<decimal> GetBalanceFromUser(string email);
       

        // ADD PURCHASE TO THE PURCHASE TABLE
        void AddPurchaseToDatabase(Purchases purchase);


        // ADD PURCHASE TO THE PURCHASE TABLE
        void AddSaleToDatabase(Sales sale);


        // UPDATE PORTFOLIO ENTRY FOR A GIVEN COIN
        void UpdatePortfolioEntryForOneCoin(Portfolio portfolio);


        // UPDATE USER BALANCE DURING PURCHASE
        Task UpdateUserBalancePurchase(string email, decimal price, int quantity);


        // UPDATE USER BALANCE DURING SALE
        Task UpdateUserBalanceSale(string email, decimal price, int quantity);


        // RETRIEVE DUMMY DATA
        List<Coins> RetrieveDummyData();


        // GET COINS IN PORTFOLIO
        List<string[]> CoinsInPortfolio(string email);


        // RETRIEVE COIN DATA FROM ALPHAVANTAGE API
        Coins GetCryptoData(string from_currency);


        // UPDATE INDIVDUAL COIN DATA
        void updateCoinData(Coins coin);


        // UPDATE COIN DATA IN DATABASE
        void DbUpdate();

    }
}
