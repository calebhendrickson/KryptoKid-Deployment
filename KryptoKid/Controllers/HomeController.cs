using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KryptoKid.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using KryptoKid.Services;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace KryptoKid.Controllers
{
    public class HomeController : Controller
    {
        
        private readonly AppDbContext _db;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeController(ILogger<HomeController> logger, AppDbContext db, UserManager<ApplicationUser> userManager, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [AllowAnonymous]
        //[Authorize]
        public async Task<ActionResult> Index()
        {
            // WEB API CODE --------------------------
            // PUT THIS IN ITS OWN CONTROLLER... IF WE EVEN KEEP IT

            //CryptoModel cryptoData = new CryptoModel
            //{
            //};
            //List<CryptoModel> cryptoList = new List<CryptoModel>();
            //string[] cryptoArray = new string[4];
            //cryptoArray[0] = "BTC";
            //cryptoArray[1] = "ETH";
            //cryptoArray[2] = "XRP";
            //cryptoArray[3] = "BCH";
            ////cryptoArray[4] = "LTC";


            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri("http://localhost:59761/api/");

            //    for(int i = 0; i < cryptoArray.Length; i++)
            //    {
            //        // HTTP GET 
            //        var responseTask = client.GetAsync("apicryptodata/" + cryptoArray[i]);
            //        responseTask.Wait();

            //        var result = responseTask.Result;
            //        if (result.IsSuccessStatusCode)
            //        {
            //            var readTask = result.Content.ReadAsAsync<CryptoModel>();
            //            readTask.Wait();

            //            cryptoData = readTask.Result;

            //        }
            //        cryptoList.Add(cryptoData);
            //    }

            //}


            
            // GETTING USERNAME/EMAIL FROM THE REQUEST COOKIE
            string cookieValueFromContext = _httpContextAccessor.HttpContext.User.Identity.Name;
            string username = cookieValueFromContext;

            // LOGGED VS NOT LOGGED IN USE CASE
            string emailid;
            ApplicationUser user;
            if(username == null)
            {
                emailid = "null";
            }
            else
            {
                user = await _userManager.FindByNameAsync(username);
                emailid = user.Email;
            }


            // RETRIEVE DUMMY DATA
            List<Coins> cryptoList = _userService.RetrieveDummyData();


            // INITIALIZING ARRAYS TO RECEIVE MODEL ARRAY VALUES
            string[] Code = new string[cryptoList.Count];
            string[] Name = new string[cryptoList.Count];
            decimal[] Rate = new decimal[cryptoList.Count];
            string[] Currency = new string[cryptoList.Count];
            DateTime[] Time = new DateTime[cryptoList.Count];

            // TRANSFERRING COINS VALUES FROM COIN MODEL ARRAY TO STRING ARRAYS 
            for (int i = 0; i < cryptoList.Count; i++)
            {

                Code[i] = cryptoList[i].From_Currency_Code;
                Name[i] = cryptoList[i].From_Currency_Name;
                Rate[i] = (decimal)cryptoList[i].Exchange_Rate;
                Currency[i] = cryptoList[i].To_Currency_Code;
                Time[i] = (DateTime)cryptoList[i].Last_Refreshed;
            }

            // INITIALIZING VIEWBAG OBJECT PROPERTIES
            ViewBag.CodeCollection = new string[cryptoList.Count];
            ViewBag.NameCollection = new string[cryptoList.Count];
            ViewBag.RateCollection = new decimal[cryptoList.Count];
            ViewBag.CurrencyCollection = new string[cryptoList.Count];
            ViewBag.TimeCollection = new DateTime[cryptoList.Count];

            // ASSIGNING VALUES TO VIEWBAG OBJECT
            ViewBag.CodeCollection = Code;
            ViewBag.NameCollection = Name;
            ViewBag.RateCollection = Rate;
            ViewBag.CurrencyCollection = Currency;
            ViewBag.TimeCollection = Time;
            ViewBag.Email = emailid;
            return View();
            
        }

        [HttpPost]
        [Authorize]
        //[AllowAnonymous]
        public async Task<ActionResult> Purchase(IFormCollection dataObject)
        {

            //List<string> dataObject = new List<string>();
            //dataObject.Add(data);
            //string stock_name = dataObject[0];
            ////string poop = data;
            //int d2 = int.Parse(dataObject[1]);
            //int d3 = int.Parse(dataObject[2]);
            //string email = dataObject[3];
            //var d5 = dataObject[4];
            string stock_name = dataObject["stock_name"];
            decimal d2 = decimal.Parse(dataObject["price"]);
            int d3 = int.Parse(dataObject["quantity"]);
            string email = dataObject["email"];
            int d5 = int.Parse(dataObject["userid"]);


            // GET THE USER ID FROM THE EMAIL
            string userid = await _userService.GetUserId(email);


            // NEED TO RETRIEVE(GET) PORTFOLIO RECORD QUANTITY
            int coinQuantity = _userService.RetrieveCoinQuantity(email, stock_name);


            // GET USER BALANCE
            decimal previousBalance = await _userService.GetBalanceFromUser(email);


            if (d2 * d3 > previousBalance)
            {
                Debug.WriteLine("insufficent funds");
                return View();
            }

            Portfolio portfolioModel = new Portfolio
            {
                id = 0,
                stock_name = stock_name,
                price = d2,
                quantity = coinQuantity + d3,
                email = email,
                userid = userid,
            };

            Purchases purchaseModel = new Purchases
            {
                id = 0,
                userid = (string)portfolioModel.userid,
                stock_name = portfolioModel.stock_name,
                price = portfolioModel.price,
                // NEED TO FIGURE OUT HOW TO CHANGE THIS-- currently saving the total owned
                quantity = (int)portfolioModel.quantity
            };

            if (portfolioModel == null)
            {
                Debug.WriteLine("model null");
            }


            // ADD PURCHASE TO THE PURCHASE TABLE
            _userService.AddPurchaseToDatabase(purchaseModel);

            // UPDATE PORTFOLIO ENTRY FOR A GIVEN COIN
            _userService.UpdatePortfolioEntryForOneCoin(portfolioModel);

            // UPDATE USER BALANCE FOR PURCHASE
            await _userService.UpdateUserBalancePurchase(email, d2, d3);
            

            // TODO redirect to account/portfolio page
            return RedirectToAction("Index", "Dashboard");
            // return View("~/Views/Account/Index.aspx", portfolioModel);
            //return View("../Account/Index");
            //return View("Index");
        }
    

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
