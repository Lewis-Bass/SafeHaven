using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebDataLayer.Models;
using WebLibraries.OptIn;

namespace WebSiteCustomer.Controllers
{
    public class OptOutController : Controller
    {
        private readonly IStringLocalizer<OptOutController> _localizer;

        public OptOutController(IStringLocalizer<OptOutController> localizer)
        {

        }

        #region Opt Out

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            // load the model
            ClaimsPrincipal currentUser = this.User;
            var optin = new OptInProcessing();
            var model = optin.LoadData(currentUser.Identity.Name);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(OptOutData model)
        {
            // save the model 
            ClaimsPrincipal currentUser = this.User;
            var optin = new OptInProcessing();
            optin.SaveData(model, currentUser.Identity.Name);

            // redirect to ??
            return View(model);
        }

        #endregion

        #region View Data Collected

        [Authorize]
        [HttpGet]
        public IActionResult View()
        {
            // load the model
            ClaimsPrincipal currentUser = this.User;
            var optin = new OptInProcessing();
            var model = optin.ViewData(currentUser.Identity.Name);
            return View(model);
        }
        
        #endregion
    }
}