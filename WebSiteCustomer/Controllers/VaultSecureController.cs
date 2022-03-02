using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using WebDataLayer.Models;
using WebLibraries.License;
//using WebLibraries.License;

namespace WebSiteCustomer.Controllers
{
    public class VaultSecureController : Controller
    {
        private readonly IStringLocalizer<VaultSecureController> _localizer;

        public VaultSecureController(IStringLocalizer<VaultSecureController> localizer)
        {

        }

        #region main page

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            VaultSecure model = GetVaultSecureModel();
            return View(model);
        }

        #endregion

        #region Device changes

        [Authorize]
        [HttpPost]
        public IActionResult VaultSecureDeviceOffline(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var m = new Maintenance();
                id = System.Net.WebUtility.UrlDecode(id);
                
                if (!m.DisableDeviceID(id))
                {
                    // handle exception
                }
            }

            var model = GetVaultSecureModel();
            return View("Index", model);
        }

        [Authorize]
        [HttpPost]
        public IActionResult VaultSecureDeviceActivate(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var m = new Maintenance();
                id = System.Net.WebUtility.UrlDecode(id);

                if (!m.ActivateDeviceID(id))
                {
                    // handle exception
                }
            }

            var model = GetVaultSecureModel();
            return View("Index", model);
        }

        #endregion

        #region Vault Change

        [Authorize]
        [HttpPost]        
        public IActionResult VaultSecureArkOffline(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var m = new Maintenance();
                id = System.Net.WebUtility.UrlDecode(id);

                if (!m.DisableArkID(id))
                {
                    // handle exception
                }
            }

            var model = GetVaultSecureModel();
            return View("Index", model);
        }

        
        [Authorize]
        [HttpPost]
        public IActionResult VaultSecureArkActivate(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var m = new Maintenance();
                id = System.Net.WebUtility.UrlDecode(id);

                if (!m.ActivateArkID(id))
                {
                    // handle exception
                }
            }

            var model = GetVaultSecureModel();
            return View("Index", model);

        }

        #endregion

        #region Helpers

        private VaultSecure GetVaultSecureModel()
        {
            // add the information from our user account
            var currentUser = this.User;
            VaultSecure model = new VaultSecure();
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var license = dbms.License.FirstOrDefault(r => r.Email == currentUser.Identity.Name);
                if (license != null)
                {
                    model.LicenseEmail = license.Email;
                    model.LicenseName = license.Name;
                    model.LicensePhoneNumber = license.PhoneNumber;
                    model.LicenseMaxDevicesSupported = license.MaxDevicesSupported.Value;
                    model.DeviceInfo = dbms.Device
                        .Include(r => r.DeviceArk)
                        .Where(r => r.LicenseId == license.LicenseId)
                        .ToArray();
                }
            }

            return model;
        }

        #endregion
    }
}