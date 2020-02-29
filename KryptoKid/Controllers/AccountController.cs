using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KryptoKid.Models;
using System.Net.Mail;
using System.Net;
using System.Web;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace KryptoKid.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            
        }
        
        // GET REGISTER FORM
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // REGISTER ACTION- POST TO DATABASE
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var username = await _userManager.FindByNameAsync(model.username);

                if (username != null)
                {
                    return View();
                }
                else
                {

                    var user = new ApplicationUser { UserName = model.username, Email = model.email };
                    var result = await _userManager.CreateAsync(user, model.userPassword);

                    // Update balance
                    var balance = await _userManager.FindByEmailAsync(model.email);
                    balance.Balance = 50000;
                    await _userManager.UpdateAsync(balance);

                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            // we want to rerender view and show errors so we need to pass the model
            return View(model);
        }

        // GET LOGIN FORM
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // LOGIN ACTION
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.email);


                var result = await _signInManager.PasswordSignInAsync(user.UserName,
                   model.password, model.rememberMe, false);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)){
                        // Local Redirect protects against open redirect vulnerability (cant be redirected to another website)
                        return Redirect(returnUrl);
                    }
                    else {
                        return RedirectToAction("Index", "Home");
                    }
                }
            
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }

        // LOGOUT ACTION & REDIRECT
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost][HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> isEmailInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if(user == null)
            {
                // jquery validate expects a json response
                return Json(true);
            }
            else
            {
                // c# 6 string interpolation
                return Json($"Email {email} is already in use");
            }
            
        }










        [NonAction]
        public void SendVerificationLinkEmail(string email, string activationCode)
        {
            var verifyUrl = "/User/VerifyAccount/" + activationCode;
            //var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);
            var link = Request.Scheme + "://" + Request.Host + verifyUrl;

            var fromEmail = new MailAddress("chendric0312@gmail.com", "chendric0312");
            var toEmail = new MailAddress(email);

            var fromEmailPassword = "Peasoup312";
            var subject = "Your account is successfully created";

            string body = "<br></br>After our extensive vetting process, we are excited to" +
                "tell you that your account has been created. Please click on the link below" +
                "to verify your account" + "<br/><br/><a href='" + link + "'>" + link + "</a>";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })

                smtp.Send(message);
        }

    }
}