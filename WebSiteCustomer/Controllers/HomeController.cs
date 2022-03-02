using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebDataLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Localization;
using SharedResourceStrings.Resources;

namespace WebSiteCustomer.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStringLocalizer<SharedResources> _localizer;

        #region home controller

        public HomeController(
            IStringLocalizer<SharedResources> localizer,
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager            
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _localizer = localizer;            
        }

        [Authorize]
        public IActionResult Index()
        {
            var xxx = _localizer.GetAllStrings();
            ClaimsPrincipal currentUser = this.User;
            var check = new WebLibraries.HealthCheck.Check();
            var healthCheck = check.GetHealthForUser(currentUser.Identity.Name);
            return View(healthCheck);
        }

        #endregion

        #region Language selection

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        #endregion

        #region error default
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #endregion
    }
}
