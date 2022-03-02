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
using Microsoft.Extensions.Localization;

namespace WebSiteCustomer.Controllers
{
    public class CustomerSecureController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStringLocalizer<CustomerSecureController> _localizer;
        public CustomerSecureController(UserManager<ApplicationUser> userManager,
            IStringLocalizer<CustomerSecureController> localizer)
        {
            _userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal currentUser = this.User;

            var model = new CustomerSecure();
            // add the information from our user account
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var license = dbms.License.FirstOrDefault(r => r.Email == currentUser.Identity.Name);
                if (license != null)
                {
                    model.Email = license.Email;
                    model.Name = license.Name;
                    model.PhoneNumber = license.PhoneNumber;
                }
            }

            return View(model);
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Index(Guid id, CustomerSecure model)
        {
            ClaimsPrincipal currentUser = this.User;

            var user = await _userManager.FindByEmailAsync(currentUser.Identity.Name);
            // update passwords
            if (model.Password1 == model.Password2 && !string.IsNullOrWhiteSpace(model.Password1) && !string.IsNullOrWhiteSpace(model.Password2))
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, model.Password1);
            }
            else if (model.Password1 != model.Password2 || string.IsNullOrWhiteSpace(model.Password1) || string.IsNullOrWhiteSpace(model.Password2))
            {
                model.ValidationErrors = $"The new passwords do not match. {model.Password1} - {model.Password2}";
            }
            
            // update our account
            using (var dbms = new WebDataLayer.DBMS.WebdatalayerContext())
            {
                var license = dbms.License.FirstOrDefault(r => r.Email == currentUser.Identity.Name);
                if ((license != null) && ((license.Name != model.Name) || (license.PhoneNumber != model.PhoneNumber)))
                {
                    license.Name = model.Name;
                    license.PhoneNumber = model.PhoneNumber;
                    await dbms.SaveChangesAsync();
                }
            }

            return View(model);
        }
    }
}
