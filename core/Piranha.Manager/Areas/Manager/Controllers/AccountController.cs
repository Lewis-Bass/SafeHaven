/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class AccountController : Controller
    {
        /// <summary>
        /// The current Api.
        /// </summary>
        private readonly IApi _api;

        /// <summary>
        /// The current security provider.
        /// </summary>
        private readonly ISecurity _security;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current Api</param>
        /// <param name="security">The current security provider</param>
        public AccountController(IApi api, ISecurity security)
        {
            _api = api;
            _security = security;
        }

        /// <summary>
        /// Gets the login view.
        /// </summary>
        /// <param name="returnurl">The optional return url</param>
        [Route("manager/login/{returnurl?}")]
        public IActionResult Login(string returnurl = null)
        {
            return View("Login", returnurl);
        }

        /// <summary>
        /// Signs out the user and redirects to the login page.
        /// </summary>
        /// <param name="returnurl">The optional return url</param>
        [Route("manager/logout")]
        public async Task<IActionResult> Logout(string returnurl = null)
        {
            await _security.SignOut(HttpContext);

            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Signs in the user with the given credentials.
        /// </summary>
        /// <param name="model">The login model</param>
        [HttpPost]
        [Route("manager/login")]
        public async Task<IActionResult> Login(Models.LoginModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.UserName) && !string.IsNullOrWhiteSpace(model.Password))
            {
                var result = await _security.SignIn(HttpContext, model.UserName, model.Password);

                if (result)
                {
                    if (!string.IsNullOrWhiteSpace(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Start");
                    }
                }
            }
            ViewBag.Message = "You have entered an invalid username or password ";
            return Login(model.ReturnUrl);
        }
    }
}