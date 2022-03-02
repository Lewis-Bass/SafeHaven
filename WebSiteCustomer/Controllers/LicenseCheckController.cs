using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibraries.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebDataLayer.Models;
using WebLibraries.License;

namespace WebSiteCustomer.Controllers
{
    public class LicenseCheckController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStringLocalizer<LicenseCheckController> _localizer;

        public LicenseCheckController(UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IStringLocalizer<LicenseCheckController> localizer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Ping()
        {
            return Content("LicenseCheckController is active");
        }

        /// <summary>
        /// Does the specfic ark have permission to run?
        /// Checking this will increase the expiration date of the license
        /// </summary>
        /// <param name="encryptedJson"></param>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        //[Authorize]
        public IActionResult Check()
        {
            string encryptedJson = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                encryptedJson = reader.ReadToEnd();
            }
            if (string.IsNullOrEmpty(encryptedJson))
            {
                return null;
            }

            string data = System.Net.WebUtility.HtmlDecode(encryptedJson);
            data = CommonLibraries.Encrypt_Decrypt.EncrypDecrypt.DecryptString(data);

            DateTime resp = DateTime.MinValue;
            try
            {
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<LicenseCheck>(data);
                var signInResult = _signInManager.PasswordSignInAsync(request.UserInfo.Name, request.UserInfo.Password, false, true).GetAwaiter().GetResult();
                if (signInResult.Succeeded && !signInResult.IsLockedOut)
                {
                    var user = _userManager.FindByNameAsync(request.UserInfo.Name).GetAwaiter().GetResult();
                    var validate = new LicenseValidation(user);
                    resp = validate.IsLicenseExpired(request.DeviceId, request.ArkId);
                }
            }
            catch (Exception ex)
            {
                // TODO Handle the error!
            }

            string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(resp);
            dataString = CommonLibraries.Encrypt_Decrypt.EncrypDecrypt.EncryptString(dataString);
            dataString = System.Net.WebUtility.HtmlEncode(dataString);

            return Content(dataString);
        }

        /// <summary>
        /// Does the specfic ark have permission to run?
        /// Checking this will increase the expiration date of the license
        /// </summary>
        /// <param name="encryptedJson"></param>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        //[Authorize]
        public IActionResult GetLicense()
        {
            string encryptedJson = string.Empty;
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                encryptedJson = reader.ReadToEnd();
            }
            if (string.IsNullOrEmpty(encryptedJson))
            {
                return null;
            }

            string data = System.Net.WebUtility.HtmlDecode(encryptedJson);
            data = CommonLibraries.Encrypt_Decrypt.EncrypDecrypt.DecryptString(data);

            UserLicense resp = null;
            try
            {
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<Login>(data);
                var signInResult = _signInManager.PasswordSignInAsync(request.Name, request.Password, false, true).GetAwaiter().GetResult();
                if (signInResult.Succeeded && !signInResult.IsLockedOut)
                {
                    var user = _userManager.FindByNameAsync(request.Name).GetAwaiter().GetResult();
                    var validate = new LicenseValidation(user);
                    resp = validate.GetLicense();
                }
            }
            catch (Exception ex)
            {
                // TODO Handle the exception!!
            }

            string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(resp);
            dataString = CommonLibraries.Encrypt_Decrypt.EncrypDecrypt.EncryptString(dataString);
            dataString = System.Net.WebUtility.HtmlEncode(dataString);

            return Content(dataString);
        }
    }
}