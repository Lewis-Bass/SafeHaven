using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibraries.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using WebDataLayer.Models;
using WebLibraries.Registration;

namespace WebSiteCustomer.Controllers
{
    public class LicenseRegistrationController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<LicenseRegistrationController> _localizer;

        public LicenseRegistrationController(UserManager<ApplicationUser> userManager,
            IStringLocalizer<LicenseRegistrationController> localizer)
        {            
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Ping()
        {
            return Content("LicenseRegistrationController is active");
        }

        /// <summary>
        /// A new registration is being received.
        /// Or a new ark is being added to an existing registration.
        /// </summary>
        /// <param name="encryptedJson"></param>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        public IActionResult NewRegistration()
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

            CommonLibraries.Models.GenericRestResponse resp;
            try
            {
                var request = Newtonsoft.Json.JsonConvert.DeserializeObject<RequestLicense>(data);
                var procReg = new ProcessRegistration(_userManager);
                resp = procReg.RegisterDevice(request);
            }
            catch (Exception ex)
            {
                resp = new CommonLibraries.Models.GenericRestResponse()
                {
                    ResponseString = new string[] { "Error occured - please try again later " }
                };
            }

            string dataString = Newtonsoft.Json.JsonConvert.SerializeObject(resp);
            dataString = CommonLibraries.Encrypt_Decrypt.EncrypDecrypt.EncryptString(dataString);
            dataString = System.Net.WebUtility.HtmlEncode(dataString);

            return Content(dataString);
        }
        
    }

}