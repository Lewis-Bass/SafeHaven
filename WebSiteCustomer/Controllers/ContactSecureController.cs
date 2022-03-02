using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CommonLibraries.Extensions;
using Microsoft.EntityFrameworkCore;
using WebDataLayer.Models;
using WebLibraries.Contact;
using Microsoft.Extensions.Localization;

namespace WebSiteCustomer.Controllers
{
    public class ContactSecureController : Controller
    {
        private readonly IStringLocalizer<ContactSecureController> _localizer;

        public ContactSecureController(IStringLocalizer<ContactSecureController> localizer)
        {

        }

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {

            return View(new ContactSecureData());
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(WebDataLayer.Models.ContactSecureData data)
        {
            ClaimsPrincipal currentUser = this.User;
            var contact = new ContactUs();
            data.Response = contact.ProcessContact(currentUser.Identity.Name, data);
            return View(data);
        }
    }
}