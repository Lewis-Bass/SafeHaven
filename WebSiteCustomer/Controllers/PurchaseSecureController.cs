using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebDataLayer.Models;

namespace WebSiteCustomer.Controllers
{
    public class PurchaseSecureController : Controller
    {
        private readonly IStringLocalizer<PurchaseSecureController> _localizer;

        public PurchaseSecureController(IStringLocalizer<PurchaseSecureController> localizer)
        {

        }

        [Authorize]
        [HttpGet]
        public IActionResult Index(Guid id)
        {
            // get pirahna model info
            var model = new PurchaseSecure();

            // add the information from our user account
            ClaimsPrincipal currentUser = this.User;
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var license = dbms.License.FirstOrDefault(r => r.Email == currentUser.Identity.Name);
                if (license != null)
                {
                    model.ProductName = $"Vault {license.MaxDevicesSupported}";
                    model.ProductExpirationDate = license.ExpirationDate.HasValue ?
                        license.ExpirationDate.Value : DateTime.Today;
                    model.LicenseId = license.LicenseId;
                }
            }

            return View(model);
        }

       
        [Authorize]
        [HttpPost]
        public IActionResult Index(PurchaseSecure model)
        {
            // card and security present
            if (string.IsNullOrWhiteSpace(model.CreditCard) || string.IsNullOrWhiteSpace(model.SecurityCode))
            {
                model.ErrorMessage = $"Credit Card and Security Code are Required.";
            }
            else
            {
                var purchase = new WebLibraries.CardProcessing.PurchaseLicense();
                if (purchase.Purchase(model.LicenseId, model.CreditCard, model.SecurityCode, 0))
                {
                    model.SuccessMessage = $"{model.ProductName} has been purchased";
                }
                else
                {
                    model.ErrorMessage = "Purchase can not be completed at this time - Credit Card Denied";
                }
            }

            return View(model);
        }

    }
}