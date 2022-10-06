using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopCourse.Core.DTOs;
using TopCourse.Core.Services.Interfaces;

namespace TopCourse.Web.Areas.UserPanel.Controllers
{
    [Area("UserPanel")]
    [Authorize]
    public class WalletController : Controller
    {

        private IUserService _userService;
        public WalletController(IUserService userService)
        {
            _userService = userService;
        }

        [Route("UserPanel/Wallet")]
        public IActionResult Index()
        {
            ViewData["BalaneUserWallet"] = _userService.BalanceUserWallet(User.Identity.Name);
            ViewBag.ListWallet = _userService.GetWalletUser(User.Identity.Name); //(User.Identity.Name) is current user login
            return View();
        }

        [Route("UserPanel/Wallet")]
        [HttpPost]
        public IActionResult Index(ChargeWalletViewModel charge)
        {
            if(!ModelState.IsValid)
            {
                ViewBag.ListWallet = _userService.GetWalletUser(User.Identity.Name); //(User.Identity.Name) is current user login
                return View(charge);
            }
            int walletId = _userService.ChargeWallet(User.Identity.Name, charge.Amount, "شارژ حساب",true);

            #region OnlinePayment
            //var payment = new ZarinpalSandbox.Payment(charge.Amount);
           // var response = payment.PaymentRequest("شارژ کیف پول", "https://localhost:44381/OnlinePayment/"+walletId);

            //if(response.Result.Status == 100)
            //{
             //   return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + response.Result.Authority);
            //}

            #endregion

            return Redirect("/OnlinePayment");
        }
    }
}