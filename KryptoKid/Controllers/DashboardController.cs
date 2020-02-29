using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using KryptoKid.Models;
using KryptoKid.Services;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace KryptoKid.Controllers
{
    public class DashboardController : Controller
    {

        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public DashboardController(AppDbContext db, UserManager<ApplicationUser> userManager, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userManager = userManager;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Index()
        {
            //string emailid = "chendric0312@gmail.com";
            // RETRIEVE DUMMY DATA
            List<Coins> cryptoList = _userService.RetrieveDummyData();

            decimal[] Currency = new decimal[cryptoList.Count];
            string[] Name = new string[cryptoList.Count];

            for (int i = 0; i < cryptoList.Count; i++)
            {

                Currency[i] = (decimal)cryptoList[i].Exchange_Rate;
                Name[i] = cryptoList[i].From_Currency_Name;

            }
           
            // GETTING USERNAME/EMAIL FROM THE REQUEST COOKIE
            string cookieValueFromContext = _httpContextAccessor.HttpContext.User.Identity.Name;
            string username = cookieValueFromContext;
            var user = await _userManager.FindByNameAsync(username);
            string emailid = user.Email;
            

            // GET COINS IN PORTFOLIO
            List<string[]> rows = _userService.CoinsInPortfolio(emailid);


            // CREATE ARRAYS TO BE ASSIGNED TO VIEWBAG
            int size = rows.Count();
            
            string[] NameCollection = new string[size];
            decimal[] PriceCollection = new decimal[size];
            int[] QuantityCollection = new int[size];

            for (int i = 0; i < size; i++)
            {
                NameCollection[i] = rows[i][0];
                for (int j = 0; j < Name.Length; j++)
                {
                    if (Name[j] == NameCollection[i])
                    {
                        PriceCollection[i] = decimal.Parse(rows[i][1]);
                    }
                }
                QuantityCollection[i] = int.Parse(rows[i][2]);

            }

            // CALCULATE TOTAL ASSET VALUE 
            decimal sum = 0;
            for (int i = 0; i < QuantityCollection.Length; i++)
            {
                sum = sum + PriceCollection[i] * QuantityCollection[i];
            }

            // CALCULATE THE USERS BALANCE
            decimal balanceValue = await _userService.GetBalanceFromUser(emailid);


            // INITIALIZE VIEWBAGPROPERTY ARRAYS
            ViewBag.Name = new List<string>(NameCollection);
            ViewBag.Value = new List<decimal>(PriceCollection);
            ViewBag.Shares = new int[size];

            // ASSIGN VALUES TO VIEWBAG PROPERTY ARRAYS
            ViewBag.Name = NameCollection;
            ViewBag.Value = PriceCollection;
            ViewBag.Shares = QuantityCollection;
            ViewBag.Email = emailid;
            ViewBag.Length = size;
            ViewBag.Assets = sum;
            ViewBag.Balance = balanceValue;
           

            //AccountPortfolioModel accountPortfolioModel = new AccountPortfolioModel
            //{
            //    Name = new List<string>(NameCollection),
            //    Value = new List<decimal>(PriceCollection),
            //    Shares = QuantityCollection,
            //    Email = emailid,
            //    Length = size,
            //    Assets = sum,
            //    Balance = balanceValue
            //};

            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Sell(IFormCollection dataObject)
        {
            //string stock_name = dataObject[0];
            //int d2 = int.Parse(dataObject[1]);
            //int d3 = int.Parse(dataObject[2]);
            //string emailid = dataObject[3];

            string stock_name = dataObject["stock_name"];
            decimal d2 = decimal.Parse(dataObject["price"]);
            int d3 = int.Parse(dataObject["quantity"]);
            string emailid = dataObject["email"];
            int d5 = int.Parse(dataObject["userid"]);


            // GET THE USER ID FROM THE EMAIL
            string userid = await _userService.GetUserId(emailid);

            // NEED TO RETRIEVE(GET) PORTFOLIO RECORD QUANTITY
            int coinQuantity = _userService.RetrieveCoinQuantity(emailid, stock_name);


            if (coinQuantity - d3 < 0)
            {
                Debug.WriteLine("insufficent funds/ quantity");
                return View();
            }

            Portfolio portfolioModel = new Portfolio
            {
                id = 0,
                stock_name = stock_name,
                price = d2,
                quantity = coinQuantity - d3,
                email = emailid,
                userid = userid,
            };

            Sales saleModel = new Sales
            {
                id = 0,
                userid = userid,
                stock_name = stock_name,
                price = d2,
                quantity = d3,
            };

            if (portfolioModel == null)
            {
                Debug.WriteLine("model null");
            }


            // ADD PURCHASE TO THE PURCHASE TABLE
            _userService.AddSaleToDatabase(saleModel);


            // UPDATE PORTFOLIO ENTRY FOR A GIVEN COIN
            _userService.UpdatePortfolioEntryForOneCoin(portfolioModel);


            // UPDATE USER BALANCE FOR SALE
            await _userService.UpdateUserBalanceSale(emailid, d2, d3);


            //ViewBag.Shares = portfolioModel.quantity;
            // TODO page refresh??
            //return View("Index");
            return RedirectToAction("Index", "Dashboard");

        }
    }
}