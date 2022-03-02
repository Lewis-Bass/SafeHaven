using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Piranha;
using Piranha.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CommonLibraries.Extensions;
using Microsoft.EntityFrameworkCore;

namespace WebSite.Controllers
{
    public class CmsController : Controller
    {

        #region Piranha

        private readonly IApi _api;
        private readonly IDb _db;
        private readonly UserManager<Piranha.AspNetCore.Identity.Data.User> _userManager;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="app">The current app</param>
        public CmsController(IApi api, IDb db, UserManager<Piranha.AspNetCore.Identity.Data.User> userManager)
        {
            _api = api;
            _db = db;
            _userManager = userManager;
        }

        #endregion

        #region Piranha Default Routes

        /// <summary>
        /// Gets the blog archive with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        /// <param name="year">The optional year</param>
        /// <param name="month">The optional month</param>
        /// <param name="page">The optional page</param>
        /// <param name="category">The optional category</param>
        /// <param name="tag">The optional tag</param>
        [Route("archive")]
        public async Task<IActionResult> Archive(Guid id, int? year = null, int? month = null, int? page = null,
            Guid? category = null, Guid? tag = null)
        {
            var model = await _api.Pages.GetByIdAsync<Models.BlogArchive>(id);
            model.Archive = await _api.Archives.GetByIdAsync(id, page, category, tag, year, month);

            return View(model);
        }

        /// <summary>
        /// Gets the page with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        [Route("page")]
        public async Task<IActionResult> Page(Guid id)
        {
            var model = await _api.Pages.GetByIdAsync<Models.StandardPage>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the page with the given id.
        /// </summary>
        /// <param name="id">The unique page id</param>
        [Route("pagewide")]
        public async Task<IActionResult> PageWide(Guid id)
        {
            var model = await _api.Pages.GetByIdAsync<Models.StandardPage>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the post with the given id.
        /// </summary>
        /// <param name="id">The unique post id</param>
        ///
        [Route("post")]
        public async Task<IActionResult> Post(Guid id)
        {
            var model = await _api.Posts.GetByIdAsync<Models.BlogPost>(id);

            return View(model);
        }

        /// <summary>
        /// Gets the teaser page with the given id.
        /// </summary>
        /// <param name="id">The page id</param>
        /// <param name="startpage">If this is the startpage of the site</param>
        [Route("teaserpage")]
        public async Task<IActionResult> TeaserPage(Guid id, bool startpage = false)
        {
            var model = await _api.Pages.GetByIdAsync<Models.TeaserPage>(id);

            if (startpage)
            {
                var latest = _db.Posts
                    .Where(p => p.Published <= DateTime.Now)
                    .OrderByDescending(p => p.Published)
                    .Take(1)
                    .Select(p => p.Id);
                if (latest.Count() > 0)
                {
                    model.LatestPost = await _api.Posts
                        .GetByIdAsync<PostInfo>(latest.First());
                }
                return View("startpage", model);
            }
            return View(model);
        }

        #endregion

        #region Additional Routes

        //#region PurchaseSecure

        //[Route("purchasesecure")]
        ////[Authorize(Roles = "Customer")]
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> PurchaseSecure(Guid id)
        //{
        //    // get pirahna model info
        //    var model = await _api.Pages.GetByIdAsync<Models.PurchaseSecure>(id);

        //    // add the information from our user account
        //    ClaimsPrincipal currentUser = this.User;
        //    using (var dbms = new WebDataLayer.DBMS.WebDataLayerContext())
        //    {
        //        var license = dbms.License.FirstOrDefault(r => r.Email == currentUser.Identity.Name);
        //        if (license != null)
        //        {
        //            model.ProductName = $"Vault {license.MaxDevicesSupported}";
        //            model.ProductExpirationDate = license.ExpirationDate.HasValue ?
        //                license.ExpirationDate.Value.ToDateTime() : DateTime.Today;
        //            model.LicenseId = license.LicenseId;
        //        }
        //    }

        //    return View(model);
        //}

        //[Route("purchasesecure")]
        ////[Authorize(Roles = "Customer")]
        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> PurchaseSecure(Guid id, Models.PurchaseSecure model)
        //{
        //    // card and security present
        //    if (string.IsNullOrWhiteSpace(model.CreditCard) || string.IsNullOrWhiteSpace(model.SecurityCode))
        //    {
        //        ErrorMessage($"Credit Card and Security Code are Required.", false);
        //    }
        //    else
        //    {
        //        var purchase = new WebLibraries.CardProcessing.PurchaseLicense();
        //        if (purchase.Purchase(model.LicenseId, model.CreditCard, model.SecurityCode, 0))
        //        {
        //            SuccessMessage($"{model.ProductName} has been purchased");
        //        }
        //        else
        //        {
        //            ErrorMessage("Purchase can not be completed at this time -Credit Card Denied");
        //        }
        //    }

        //    var modelOut = await _api.Pages.GetByIdAsync<Models.PurchaseSecure>(id);
        //    modelOut.ProductName = model.ProductName;
        //    modelOut.ProductExpirationDate = model.ProductExpirationDate;
        //    modelOut.LicenseId = model.LicenseId;
        //    SetupViewData();
        //    return View(modelOut);
        //}

        //#endregion

        //#region VaultSecure
        //[Route("vaultsecure")]
        ////[Authorize(Roles = "Customer")]
        //[Authorize]
        //[HttpGet]
        //public async Task<IActionResult> VaultSecure(Guid id)
        //{
        //    Models.VaultSecure model = await GetVaultSecureModel(id);
        //    return View(model);
        //}

        //private async Task<Models.VaultSecure> GetVaultSecureModel(Guid id)
        //{
        //    // get piranha model info
        //    var model = await _api.Pages.GetByIdAsync<Models.VaultSecure>(id);

        //    // add the information from our user account
        //    ClaimsPrincipal currentUser = this.User;
        //    using (var dbms = new WebDataLayer.DBMS.WebDataLayerContext())
        //    {
        //        var license = dbms.License.FirstOrDefault(r => r.Email == currentUser.Identity.Name);
        //        if (license != null)
        //        {
        //            model.PageId = id;
        //            model.LicenseEmail = license.Email;
        //            model.LicenseName = license.Name;
        //            model.LicensePhoneNumber = license.PhoneNumber;
        //            model.LicenseMaxDevicesSupported = license.MaxDevicesSupported.Value;
        //            model.DeviceInfo = dbms.Device
        //                .Include(r => r.DeviceArk)
        //                //.Include(r => r.DeviceInformation)
        //                .Where(r => r.LicenseId == license.LicenseId)
        //                .ToArray();
        //        }
        //    }

        //    return model;
        //}

        //[Route("vaultsecuredeviceoffline")]
        ////[Authorize(Roles = "Customer")]
        //[Authorize]
        //[HttpPost]
        ////public async Task<IActionResult> VaultSecureDeviceOffline(Models.VaultSecure model)
        //public async Task<IActionResult> VaultSecureDeviceOffline(string id)
        //{
        //    Guid pageId = Guid.Empty;
        //    string deviceId = string.Empty;
        //    foreach (var strIn in id.Split('|'))
        //    {
        //        var strs = strIn.Split('=');
        //        if (strs.Length >= 2)
        //        {
        //            if (strs[0].Trim() == "DeviceId")
        //            {
        //                deviceId = strs[1].Trim();
        //            }
        //            else if (strs[0] == "PageId")
        //            {
        //                Guid.TryParse(strs[1].Trim(), out pageId);
        //            }
        //        }
        //    }
            
        //    if (!string.IsNullOrWhiteSpace(deviceId))
        //    {
        //        // disable the device and arks
        //    }

        //    var model = GetVaultSecureModel(pageId);
        //    return View("VaultSecure", model);
        //}

        //[Route("vaultsecure/d eviceactivate")]
        ////[Authorize(Roles = "Customer")]
        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> VaultSecureDeviceActivate(Models.VaultSecure model)
        //{
        //    return View("VaultSecure", model);
        //}






        //[Route("vaultsecure/arkoffline")]
        ////[Authorize(Roles = "Customer")]
        //[Authorize]
        //[HttpPost]
        ////public async Task<IActionResult> VaultSecureArkOffline(Models.VaultSecure model)
        //public async Task<IActionResult> VaultSecureArkOffline(object id)
        //{
        //    //return View("VaultSecure", model);
        //    return View();
        //}

        //[Route("vaultsecure/arkactivate")]
        ////[Authorize(Roles = "Customer")]
        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> VaultSecureArkActivate(Models.VaultSecure model)
        //{
        //    return View("VaultSecure", model);
        //}




        //#endregion

        //#region helpers

        ///// <summary>
        ///// Adds a success message to the current view.
        ///// </summary>
        ///// <param name="msg">The message</param>
        ///// <param name="persist">If the message should be persisted in TempData</param>
        //protected void SuccessMessage(string msg, bool persist = true)
        //{
        //    msg = "<b>Success:</b> " + msg;

        //    ViewBag.MessageCss = "alert alert-success";
        //    ViewBag.Message = msg;
        //    if (persist)
        //    {
        //        TempData["MessageCss"] = "alert alert-success";
        //        TempData["Message"] = msg;
        //    }
        //}

        ///// <summary>
        ///// Adds an error message to the current view.
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="persist">If the message should be persisted in TempData</param>
        //protected void ErrorMessage(string msg, bool persist = true)
        //{
        //    msg = "<b>Error:</b> " + msg;

        //    ViewBag.MessageCss = "alert alert-danger";
        //    ViewBag.Message = msg;
        //    if (persist)
        //    {
        //        TempData["MessageCss"] = "alert alert-danger";
        //        TempData["Message"] = msg;
        //    }
        //}

        ///// <summary>
        ///// Adds an information message to the current view.
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <param name="persist">If the message should be persisted in TempData</param>
        //protected void InformationMessage(string msg, bool persist = true)
        //{
        //    msg = "<b>Information:</b> " + msg;

        //    ViewBag.MessageCss = "alert alert-info";
        //    ViewBag.Message = msg;
        //    if (persist)
        //    {
        //        TempData["MessageCss"] = "alert alert-info";
        //        TempData["Message"] = msg;
        //    }
        //}

        //protected void SetupViewData()
        //{
        //    if (TempData.ContainsKey("MessageCss"))
        //    {
        //        ViewBag.MessageCss = TempData["MessageCss"];
        //        TempData.Remove("MessageCss");
        //    }
        //    if (TempData.ContainsKey("Message"))
        //    {
        //        ViewBag.Message = TempData["Message"];
        //        TempData.Remove("Message");
        //    }
        //}

        //#endregion

        #endregion

    }
}
